using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Enmarcha.ImageGalery.Model;
using Enmarcha.ImageGalery.Service.Interface;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Class.Data;
using Enmarcha.SharePoint.Class.Logs;
using Microsoft.SharePoint;

namespace Enmarcha.ImageGalery.Service.Entities
{
    public  class ImageGaleryService:IImageGaleryService
    {
        public SPList List { get; set; }
        public ILog Logger { get; set; }
        public int Items { get; set; }
        public ImageGaleryService(SPList list,int items)
        {
            this.List = list;
            this.Items = items;
            this.Logger = new LogManager().GetLogger(new StackTrace().GetFrame(0));
    }
        public IList<ImageGallery> GetNews()
        {
            try
            {
                var repositorySharePoint = new SharePointRepository<ImageGallery>(this.List.ParentWeb, this.Logger,
                    this.List.Title, this.Items);
                var imageCollection = repositorySharePoint.GetAll();
                return imageCollection.ToList();
            }
            catch (Exception exception)
            {
                this.Logger.Error(string.Concat("Error GetNews",exception.Message));
                return null;
            }

        }
    }
}
