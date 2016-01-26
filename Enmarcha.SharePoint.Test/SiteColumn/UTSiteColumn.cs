using Enmarcha.SharePoint.Abstract.Interfaces;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Enmarcha.SharePoint.Test.SiteColumn
{
    [TestClass]
    public class UTSiteColumn
    {
        [TestMethod]
        public void CreateColumnSite()
        {
            var mockSiteColumn = new Mock<ISiteColumn>();
            mockSiteColumn.Setup(x => x.Create()).Returns(true);
            var siteColumn = mockSiteColumn.Object;
            Assert.IsTrue(siteColumn.Create());
        }
    }
}
