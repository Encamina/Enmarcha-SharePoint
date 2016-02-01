using System;
using System.Collections.Generic;
using System.Linq;
using Enmarcha.ImageGalery.Model;
using Enmarcha.ImageGalery.Service.Entities;
using Enmarcha.SharePoint.Helpers.Data;
using Microsoft.SharePoint;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace Enmarcha.ImageGalery.Test
{
    [TestClass]
    public class UTImageGalleryService
    {
        [TestMethod]
        public void GetNews()
        {
            var fakeSiteUrl = "http://www.telerik.com";
            var fakeSharepointSite = Mock.Create<SPSite>();
            var fakeSharePointList = Mock.Create<SPList>();
                       
            Mock.Arrange(() => SPContext.Current.Site).Returns(fakeSharepointSite);
            Mock.Arrange(() => fakeSharepointSite.RootWeb.Lists.TryGetList("demo")).Returns(fakeSharePointList);

            var service = new ImageGaleryService(fakeSharePointList, 10);            
            Mock.Arrange(() => service.GetNews()).Returns(new List<ImageGallery>
            { new ImageGallery
            {
                Title = "Imagen",
                Description = "Image",
                UrlNew = new UrlField {Description = string.Empty,Url = "http://google.es"},
                Image = new UrlField {Description = string.Empty,Url = "http://google.es"},
                Visible = true,
                ID = "1",
                OpenWindows = true
            } });
            

        }
    }
}
