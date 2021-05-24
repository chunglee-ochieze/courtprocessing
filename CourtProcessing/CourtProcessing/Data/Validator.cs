using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Events;
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
        private readonly IConfiguration _config;

        public Validator()
        {
        }

        public Validator(IConfiguration config)
        {
            _config = config;
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
                LogHandler.WriteLog(ex.Message, LogEventLevel.Error, _config);
                return false;
            }
        }

        public void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (!Enum.TryParse<XmlSeverityType>("Error", out var type))
                return;

            if (type != XmlSeverityType.Error)
                return;

            LogHandler.WriteLog(e.Message, LogEventLevel.Error, _config);
            throw new Exception(e.Message);
        }
    }
}
