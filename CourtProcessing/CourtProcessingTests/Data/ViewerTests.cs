using Microsoft.VisualStudio.TestTools.UnitTesting;
using CourtProcessing.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtProcessing.Data.Tests
{
    [TestClass]
    public class ViewerTests
    {
        [TestMethod]
        public void ViewUploadsTest()
        {
            var viewTest = new Viewer().ViewUploads();

            Assert.AreEqual(6, viewTest.Count);
        }
    }
}