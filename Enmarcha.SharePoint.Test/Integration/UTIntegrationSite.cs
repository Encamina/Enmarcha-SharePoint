using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Class.Logs;
using Enmarcha.SharePoint.Entities.Artefacts;
using Enmarcha.SharePoint.Test.Base;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Enmarcha.SharePoint.Test.Integration
{
    [TestClass]
    public class UTIntegrationSite
    {
        public SPSite SpSite;
        public ILog Logger;
        public Site Site;   

        [TestInitialize]
        public void Init()
        {
            SpSite = ContextSharePoint.CreateClientContext();
            Logger = new LogManager().GetLogger(new System.Diagnostics.StackTrace().GetFrame(0));
        }
        [TestMethod]
        public void CreateSite()
        {
            if (!ContextSharePoint.VerifyServer(SpSite)) Assert.Inconclusive();
            Site = new Site(SpSite.RootWeb,this.Logger);
           Assert.IsTrue(Site.CreateSite("Test", "Test", "Descripcion", "STS#0", 3082));

        }

        [TestMethod]
        public void CreateSiteFail()
        {
            if (!ContextSharePoint.VerifyServer(SpSite)) Assert.Inconclusive();
            Site = new Site(null, this.Logger);
            Assert.IsFalse(Site.CreateSite("Test", "Test", "Descripcion", "STS#0", 3082));
        }

        [TestMethod]
        public void Permision()
        {
            if (!ContextSharePoint.VerifyServer(SpSite)) Assert.Inconclusive();
            Site = new Site(SpSite.RootWeb, this.Logger);
            Assert.IsTrue(Site.AddPermision("Administradores de jerarquías",RoleType.Administrator));
            Assert.IsTrue(Site.RemovePermision("Administradores de jerarquías"));

        }

      
    }

}
