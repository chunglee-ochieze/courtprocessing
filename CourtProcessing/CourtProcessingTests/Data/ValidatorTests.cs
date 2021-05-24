using Microsoft.VisualStudio.TestTools.UnitTesting;
using CourtProcessing.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CourtProcessing.Data.Tests
{
    [TestClass]
    public class ValidatorTests
    {
        [TestMethod]
        public void ValidateXmlWithSchemaTest()
        {
            var xmlPath = "C:\\Users\\find\\Downloads\\archive\\party.XML";
            var xsdPath = "C:\\Users\\findc\\RepoF\\WebApps\\CourtProcessing\\CourtProcessing\\CourtProcessing\\party.xsd";
            var valid = new Validator().ValidateXmlWithSchema(xmlPath, xsdPath);
            Assert.AreEqual(true, valid);
        }
    }
}