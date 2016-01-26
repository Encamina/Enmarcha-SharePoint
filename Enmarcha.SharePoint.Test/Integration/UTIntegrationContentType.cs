using Enmarcha.SharePoint.Abstract.Interfaces;
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
    public class UtIntegrationContentType
    {
        public SPSite Site;
        public ContentType ContentType;
        public Entities.Artefacts.SiteColumn SiteColumn;
        public ILog Logger;
        [TestInitialize]
        public void Init()
        {
            Site = ContextSharePoint.CreateClientContext();
            if (ContextSharePoint.VerifyServer(Site))
            {
                SiteColumn = new Entities.Artefacts.SiteColumn(new ParamsSiteColumnBaseExtended
                {
                    AddPrefix = true,
                    FieldType = SPFieldType.Text,
                    Group = "Test",
                    MultiValue = false,
                    Name = "test1",
                    Requiered = false,
                    Web = Site.RootWeb
                });
                SiteColumn.Create();
            }
            Logger= new LogManager().GetLogger(new System.Diagnostics.StackTrace().GetFrame(0));

        }
        [TestMethod]
        public void CreateContentType()
        {
            if (!ContextSharePoint.VerifyServer(Site)) Assert.Inconclusive();
            ContentType= new ContentType(Site.RootWeb,Logger,"Nuevo Tipo Contenido1","TEST","Item");
            Assert.IsTrue(ContentType.Create(string.Empty));
            Assert.IsTrue(ContentType.Delete());
        }

        [TestMethod]
        public void CreateContentTypebyGuid()
        {
            if (!ContextSharePoint.VerifyServer(Site)) Assert.Inconclusive();
            ContentType= new ContentType(Site.RootWeb,Logger,"Nuevo tipo Contenido","TEst","Elemento");
            Assert.IsTrue(ContentType.Create("0x0100B84152A0E015D14CA1300027B66FAD1F"));
            Assert.IsTrue(ContentType.Delete());
        }
        [TestMethod]
        public void AddFieldContentType()
        {
            if (!ContextSharePoint.VerifyServer(Site)) Assert.Inconclusive();
            ContentType = new ContentType(Site.RootWeb, Logger, "Nuevo Tipo Contenido1", "TEST", "Item");
            Assert.IsTrue(ContentType.Create(string.Empty));

            Assert.IsTrue(ContentType.AddColumn(SiteColumn.Name));
            Assert.IsTrue(ContentType.Delete());
        }

        [TestMethod]
        public void RemoveFieldContentType()
        {
            if (!ContextSharePoint.VerifyServer(Site)) Assert.Inconclusive();
            ContentType = new ContentType(Site.RootWeb, Logger, "Nuevo Tipo Contenido1", "TEST", "Item");
            Assert.IsTrue(ContentType.Create(string.Empty));
            Assert.IsTrue(ContentType.AddColumn(SiteColumn.Name));
            Assert.IsTrue(ContentType.RemoveColumn(SiteColumn.Name));
            Assert.IsTrue(ContentType.Exist());
            Assert.IsTrue(ContentType.Delete());
        }

        [TestCleanup]
        public void CleanUp()
        {
            if (ContextSharePoint.VerifyServer(Site))
            {
                SiteColumn.Delete();
                Site.Dispose();
            }

        }
    }
}
