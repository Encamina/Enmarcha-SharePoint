using System;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Enmarcha.ImageGalery.Service.Entities;
using Enmarcha.ImageGalery.SP.Helper;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Class.Logs;
using Microsoft.SharePoint;

namespace Enmarcha.ImageGalery.SP.WebPart.ImageGalleryWebPart
{
    public partial class ImageGalleryWebPartUserControl : UserControl
    {
       public ILog Logger=  new LogManager().GetLogger(new StackTrace().GetFrame(0));
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            try
            {
                var listSharePoint = SPContext.Current.Web.Lists.TryGetList(Constants.List.ImageGallery);
                var imageGaleryService = new ImageGaleryService(listSharePoint, 5);
                var imageGaleryCollection = imageGaleryService.GetNews();
                listViewImageGalery.DataSource = imageGaleryCollection;
                listViewImageGalery.DataBind();
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Concat LoadData",exception.Message));
            }
        }
    }
}
