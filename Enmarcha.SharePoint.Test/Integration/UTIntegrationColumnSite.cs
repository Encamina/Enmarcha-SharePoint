using System.Collections.Specialized;
using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Helpers.SiteColumn;
using Enmarcha.SharePoint.Test.Base;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Enmarcha.SharePoint.Test.Integration
{
    [TestClass]
    public class UTIntegrationColumnSite
    {
        public SPSite site;
        private Entities.Artefacts.SiteColumn siteColumn;
        [TestInitialize]
        public void Init()
        {
            site = ContextSharePoint.CreateClientContext();            

        }
        [TestMethod]
        public void CreateColumnSiteText()
        {
         if (!ContextSharePoint.VerifyServer(site)) Assert.Inconclusive();
          var  siteColumn = new Entities.Artefacts.SiteColumn(new ParamsSiteColumnDefaultValue
            {
                Web = site.RootWeb,
                AddPrefix = true,
                FieldType = SPFieldType.Text,
                Group = "TEST",
                MultiValue = false,
                Name = "ColumnText",
                Requiered = false,
                DefaultValue = "GIKA"
            });
            Assert.IsTrue(siteColumn.Create());
            Assert.IsTrue(siteColumn.Delete()); 
        }
        [TestMethod]
        public void CreateColumnSiteBoolean()
        {
            if (!ContextSharePoint.VerifyServer(site)) Assert.Inconclusive();            
            siteColumn = new Entities.Artefacts.SiteColumn(new ParamsSiteColumnBaseExtended
            {
                AddPrefix = true,
                FieldType = SPFieldType.Boolean,
                Group = "TEST",
                MultiValue = false,
                Name = "ColumnBoolean",
                Requiered = false,
                Web = site.RootWeb

            });
            Assert.IsTrue(siteColumn.Create());
            siteColumn = new Entities.Artefacts.SiteColumn(new ParamsSiteColumnBase
            {
                AddPrefix = true,
                Name = "ColumnBoolean",
                Web = site.RootWeb
            });
            Assert.IsTrue(siteColumn.Exist());
            Assert.IsTrue(siteColumn.Delete());
        }
        [TestMethod]
        public void CreateColumnSiteChoice()
        {
            if (!ContextSharePoint.VerifyServer(site)) Assert.Inconclusive();
            siteColumn = new Entities.Artefacts.SiteColumn(new ParamsSiteColumnChoices
            {
                Web = site.RootWeb,
                AddPrefix = true,
                FieldType = SPFieldType.Choice,
                Group = "TEST",
                MultiValue = false,
                Name = "ColumnChoice",
                Requiered = false,
                Choices = new StringCollection {"one","thow"}
                
            });
            Assert.IsTrue(siteColumn.Create());
            Assert.IsTrue(siteColumn.Delete());
        }
          [TestMethod]
        public void CreateColumnSiteChoiceParams()
        {
            if (!ContextSharePoint.VerifyServer(site)) Assert.Inconclusive();
            siteColumn = new Entities.Artefacts.SiteColumn(new ParamsSiteColumnChoices
            {
                Web = site.RootWeb,
                AddPrefix = true,
                FieldType = SPFieldType.Choice,
                Group = "TEST",
                MultiValue = false,
                Name = "ColumnChoice",
                Requiered = false,
                Choices = new StringCollection {"one","thow"}
                
            },"thow",true);
            Assert.IsTrue(siteColumn.Create());
            Assert.IsTrue(siteColumn.Delete());
        }
        [TestMethod]
        public void CreateColumnSiteDateTime()
        {
            if (!ContextSharePoint.VerifyServer(site)) Assert.Inconclusive();
            siteColumn = new Entities.Artefacts.SiteColumn(new ParamsSiteColumnBaseExtended
            {
                Web = site.RootWeb,
                AddPrefix = true,
                FieldType = SPFieldType.DateTime,
                Group = "TEST",
                MultiValue = false,
                Name = "ColumnDateTime",
                Requiered = false              
            },TypeDate.Date);
            Assert.IsTrue(siteColumn.Create());
            Assert.IsTrue(siteColumn.Delete());
        }
        [TestMethod]
        public void CreateColumnSiteUser()
        {
            if (!ContextSharePoint.VerifyServer(site)) Assert.Inconclusive();
            siteColumn = new Entities.Artefacts.SiteColumn(new ParamsSiteColumnBaseExtended
            {
                Web = site.RootWeb,
                AddPrefix = true,
                FieldType = SPFieldType.User,
                Group = "TEST",
                MultiValue = false,
                Name = "ColumnDateTime",
                Requiered = false
            });
            Assert.IsTrue(siteColumn.Create());
            Assert.IsTrue(siteColumn.Delete());
        }

        [TestMethod]
        public void CreateColumnSiteCurrency()
        {
            if (!ContextSharePoint.VerifyServer(site)) Assert.Inconclusive();
            siteColumn = new Entities.Artefacts.SiteColumn(new ParamsSiteColumnBaseExtended()
            {
                Web = site.RootWeb,
                AddPrefix = true,
                FieldType = SPFieldType.Currency,
                Group = "TEST",
                MultiValue = false,
                Name = "ColumnChoice",
                Requiered = false,
            },2);
            Assert.IsTrue(siteColumn.Create());
            Assert.IsTrue(siteColumn.Exist());
            Assert.IsTrue(siteColumn.Delete());
        }

        [TestMethod]
        public void CreateColumnSiteCalculated()
        {
            if (!ContextSharePoint.VerifyServer(site)) Assert.Inconclusive();
            siteColumn = new Entities.Artefacts.SiteColumn(new ParamsSiteColumnBaseExtended()
            {
                Web = site.RootWeb,
                AddPrefix = true,
                FieldType = SPFieldType.Calculated,
                Group = "TEST",
                MultiValue = false,
                Name = "ColumnCalculated",
                Requiered = false,
            },"Title");
            Assert.IsTrue(siteColumn.Create());
            Assert.IsTrue(siteColumn.Exist());
            Assert.IsTrue(siteColumn.Delete());
        }

        [TestMethod]
        public void CreateColumnSiteTextRename()
        {
            if (!ContextSharePoint.VerifyServer(site)) Assert.Inconclusive();
            siteColumn = new Entities.Artefacts.SiteColumn(new ParamsSiteColumnBaseExtended
            {
                AddPrefix = true,
                FieldType = SPFieldType.Text,
                Group = "TEST",
                MultiValue = false,
                Name = "ColumnText",
                Requiered = false,
                Web = site.RootWeb

            });
            Assert.IsTrue(siteColumn.Create());
            Assert.IsTrue(siteColumn.RenameField("ENMARCHAColumnText","HOLA"));
            Assert.IsTrue(siteColumn.Exist());
            Assert.IsTrue(siteColumn.Delete());
        }
        [TestMethod]
        public void CreateColumnSiteTextHidden()
        {
            if (!ContextSharePoint.VerifyServer(site)) Assert.Inconclusive();
            siteColumn = new Entities.Artefacts.SiteColumn(new ParamsSiteColumnBaseExtended
            {
                AddPrefix = true,
                FieldType = SPFieldType.Text,
                Group = "TEST",
                MultiValue = false,
                Name = "ColumnText",
                Requiered = false,
                Web = site.RootWeb

            });
            Assert.IsTrue(siteColumn.Create());
            Assert.IsTrue(siteColumn.Hidden());                        
            Assert.IsTrue(siteColumn.Delete());
        }
        [TestCleanup]
        public void CleanUp()
        {
            if (ContextSharePoint.VerifyServer(site))
            {
                site.Dispose();
            }
        }
    }
}
