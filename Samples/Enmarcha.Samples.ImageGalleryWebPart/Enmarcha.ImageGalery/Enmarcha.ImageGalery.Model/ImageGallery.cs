using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Attribute;
using Enmarcha.SharePoint.Helpers.Data;

namespace Enmarcha.ImageGalery.Model
{
    public class ImageGallery
    {
        [Enmarcha(AddPrefeix = false, Create = false, Type = TypeField.Text)]
        public string ID { get; set; }
        [Enmarcha(AddPrefeix = false, Create = false, Type = TypeField.Text)]
        public string Title { get; set; }
        [Enmarcha(AddPrefeix = false, Create = true, Type = TypeField.Boolean, DisplayName = "Visible in ImageGallery")]
        public bool Visible { get; set; }
        [Enmarcha(AddPrefeix = false, Create = true, Type = TypeField.Boolean, DisplayName = "Open Link in new Page")]
        public bool OpenWindows { get; set; }
        [Enmarcha(AddPrefeix = false, Create = true, Type = TypeField.Url, DisplayName = "Url of image")]
        public UrlField Image { get; set; }
        [Enmarcha(AddPrefeix = false, Create = true, Type = TypeField.Url, DisplayName = "Url of new")]
        public UrlField UrlNew { get; set; }

        [Enmarcha(AddPrefeix = false, Create = true, Type = TypeField.Text, DisplayName = "Descripcion")]
        public string Description { get; set; }

        public string NewUrl
        {
            get { return UrlNew.Url; }            
        }

        public string PictureUrl
        {
            get { return Image.Url; }
        }

        public string TargetBlank
        {
            get { return (OpenWindows == true) ? " target='_blank'" : string.Empty; }
        }
    }
}
