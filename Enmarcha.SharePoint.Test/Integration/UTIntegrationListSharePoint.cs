using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Class.Logs;
using Enmarcha.SharePoint.Entities.Artefacts;
using Enmarcha.SharePoint.Helpers.SiteColumn;
using Enmarcha.SharePoint.Test.Base;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enmarcha.SharePoint.Test.Integration
{
    [TestClass]
    public class UTIntegrationListSharePoint
    {
        public SPSite Site;
        public ILog Logger;
        public ListSharePoint ListSharePoint;

        [TestInitialize]
        public void Init()
        {
            Site = ContextSharePoint.CreateClientContext();
            Logger = new LogManager().GetLogger(new System.Diagnostics.StackTrace().GetFrame(0));
        }        

        [TestMethod]
        public void CreateList()
        {
            if (!ContextSharePoint.VerifyServer(Site)) Assert.Inconclusive();
            ListSharePoint= new ListSharePoint(Site.RootWeb,Logger,"LISTATEST");
           Assert.IsTrue(ListSharePoint.Create("descripcion", ListTemplateType.Contacts, true));
            Assert.IsTrue(ListSharePoint.Delete());
        }

        [TestMethod]
        public void CreateListAddContentType()
        {
            if (!ContextSharePoint.VerifyServer(Site)) Assert.Inconclusive();
            var contentType= new ContentType(Site.RootWeb,this.Logger,"TESTHELLO","TEST","Elemento");
            contentType.Create(string.Empty);
            ListSharePoint = new ListSharePoint(Site.RootWeb, Logger, "LISTATEST");
            Assert.IsTrue(ListSharePoint.Create("descripcion", ListTemplateType.Contacts, true));
          Assert.IsTrue(ListSharePoint.AddContentType("TESTHELLO"));
          Assert.IsTrue(ListSharePoint.DeleteContentType("TESTHELLO"));
            Assert.IsFalse(ListSharePoint.AddContentType("TESTBYE"));
            Assert.IsTrue(ListSharePoint.Delete());
        }


        [TestMethod]
        public void CreateListAddField()
        {
            if (!ContextSharePoint.VerifyServer(Site)) Assert.Inconclusive();
            var contentType = new ContentType(Site.RootWeb, this.Logger, "TESTHELLO", "TEST", "Elemento");
            contentType.Create(string.Empty);
            ListSharePoint = new ListSharePoint(Site.RootWeb, Logger, "LISTATEST");
            Assert.IsTrue(ListSharePoint.Create("descripcion", ListTemplateType.Contacts, true));
            var siteColumn = new Entities.Artefacts.SiteColumn(new ParamsSiteColumnBaseExtended
            {
                AddPrefix = false,
                FieldType = SPFieldType.Text,
                Group = "Lista",
                Logger = this.Logger,
                MultiValue = false,
                Name = "Field1",
                Requiered = true,
                Web = Site.RootWeb
            });
           Assert.IsTrue(ListSharePoint.AddField(siteColumn));
            Assert.IsTrue(ListSharePoint.DeleteField(siteColumn));
            Assert.IsTrue(ListSharePoint.Delete());
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (ContextSharePoint.VerifyServer(Site))
            {
                Site.Dispose();
            }
        }
    }
}
