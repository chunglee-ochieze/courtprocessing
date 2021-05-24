using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace CourtProcessing.Data
{
    public class Validator
    {
        private readonly ILogger<Program> _logger;

        public Validator()
        {
        }

        public Validator(ILogger<Program> logger)
        {
            _logger = logger;
        }

        public bool ValidateXmlWithSchema(string xmlPath, string xsdPath)
        {
            try
            {
                var schemaSet = new XmlSchemaSet();
                schemaSet.Add("", xsdPath);
                
                using var xmlReader = XmlReader.Create(xmlPath);
                var xDocument = XDocument.Load(xmlReader);
                xDocument.Validate(schemaSet, ValidationEventHandler);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return false;
            }
        }

        public void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (!Enum.TryParse<XmlSeverityType>("Error", out var type))
                return;

            if (type != XmlSeverityType.Error)
                return;

            _logger.LogError(null, e.Message);
            throw new Exception(e.Message);
        }
    }
}
