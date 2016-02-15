using System;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Class.Logs;
using Enmarcha.SharePoint.Extensors;
using Enmarcha.SharePoint.Test.Base;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enmarcha.SharePoint.Test.Integration
{
    [TestClass]
    public class UTIntegrationExtensors
    {
        public SPSite SpSite;
        public ILog Logger;
        public UTIntegrationExtensors()
        {
            SpSite = ContextSharePoint.CreateClientContext();
            Logger = new LogManager().GetLogger(new System.Diagnostics.StackTrace().GetFrame(0));

        }
        [TestMethod]
        public void PropertyBug()
        {
            if (!ContextSharePoint.VerifyServer(SpSite)) Assert.Inconclusive();
            SPWeb web = SpSite.RootWeb;
            Assert.IsTrue(web.SetPropertyBag("hola11","World"));
            Assert.AreEqual(web.GetPropertyBag("hola11"),"World");
        }
    }
}
