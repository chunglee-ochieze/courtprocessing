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
    public class EmailHandlerTests
    {
        [TestMethod]
        public void SendEmailTest()
        {
            var sent = new EmailHandler().SendEmail(new MailModel
            {
                To = "chunglee.ochieze@yahoo.co.uk"
            });

            Assert.AreEqual(false, sent);
        }
    }
}