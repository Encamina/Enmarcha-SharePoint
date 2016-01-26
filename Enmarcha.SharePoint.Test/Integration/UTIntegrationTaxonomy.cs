using System.Linq;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Class.Logs;
using Enmarcha.SharePoint.Entities.Artefacts;
using Enmarcha.SharePoint.Test.Base;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enmarcha.SharePoint.Test.Integration
{
    [TestClass]
    public class UTIntegrationTaxonomy
    {
        public SPSite Site;
        public ILog Logger;
        public Taxonomy Taxonomy;
        [TestInitialize]
        public void Init()
        {
            Site = ContextSharePoint.CreateClientContext();
            Logger = new LogManager().GetLogger(new System.Diagnostics.StackTrace().GetFrame(0));

        }
        [TestMethod]
        public void AddGroup()
        {
            if (!ContextSharePoint.VerifyServer(Site)) Assert.Inconclusive();
            Taxonomy= new Taxonomy(this.Site,this.Logger,"Managed Metadata Service");
            Assert.IsTrue(Taxonomy.AddGroup("Test"));            
        }
        [TestMethod]
        public void AddGroupFail()
        {
            if (!ContextSharePoint.VerifyServer(Site)) Assert.Inconclusive();
            Taxonomy = new Taxonomy(this.Site, this.Logger, string.Empty);
            Assert.IsFalse(Taxonomy.AddGroup("Test"));
        }
        [TestMethod]
        public void AddTerm()
        {
            if (!ContextSharePoint.VerifyServer(Site)) Assert.Inconclusive();
            Taxonomy = new Taxonomy(this.Site, this.Logger, "Managed Metadata Service","Test");
            Assert.IsTrue(Taxonomy.AddTerms("Adrian"));            
        }
        [TestMethod]
        public void GetTerm()
        {
            if (!ContextSharePoint.VerifyServer(Site)) Assert.Inconclusive();
            Taxonomy = new Taxonomy(this.Site, this.Logger, "Managed Metadata Service", "Test");
            Assert.IsTrue(Taxonomy.GetAllTerms().Any());
            Assert.IsTrue(Taxonomy.GetTerms("Test").Any());
            Assert.IsTrue(Taxonomy.GetSubTerms("Test","Adrian").Count==0);
        }
    }
}
