using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Enmarcha.ImageGalery.Model;
using Enmarcha.ImageGalery.SP.Helper;
using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Class.Data;
using Enmarcha.SharePoint.Class.Logs;
using Enmarcha.SharePoint.Extensors;
using Enmarcha.SharePoint.Helpers.Data;
using Microsoft.SharePoint;

namespace Enmarcha.ImageGalery.SP.Features.Main
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("3b363199-07a7-4a22-a468-6b0dfd24989a")]
    public class MainEventReceiver : SPFeatureReceiver
    {
        // Uncomment the method below to handle the event raised after a feature has been activated.

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            var site = properties.Feature.Parent as SPSite;
            var web = site.RootWeb;
            ILog log = new LogManager().GetLogger(new StackTrace().GetFrame(0)); ;
            var columnSiteCollection = web.CreateColumnSite("Image Galery", typeof(ImageGallery));
            web.CreateContentType(Constants.ContentType.ImageGallery, "Enmarcha ContentType", "Elemento", columnSiteCollection);
            web.CreateList(Constants.List.ImageGallery, "Lista de la galeria de imagenes", TypeList.GenericList, true);
            var list = web.Lists.TryGetList(Constants.List.ImageGallery);
            if (list != null)
            {
                list.AddContentTypeLibrary("Image Galery");
                var repository = new SharePointRepository<ImageGallery>(web, log, Constants.List.ImageGallery, 40);
                IList<ImageGallery> collection = new List<ImageGallery>
                {
                    new ImageGallery
                {
                    Description = string.Empty,
                    Image = new UrlField { Description = "Image", Url = "/Style%20Library/Images/01.jpg"},
                    UrlNew = new UrlField { Description = "New", Url = "/Style%20Library/Images/01.jpg" },
                    OpenWindows = true,
                    Visible = true
                },
                       new ImageGallery
                {
                    Description = string.Empty,
                    Image = new UrlField { Description = "Image", Url = "/Style%20Library/Images/02.jpg"},
                    UrlNew = new UrlField { Description = "New", Url = "/Style%20Library/Images/02.jpg" },
                    OpenWindows = true,
                    Visible = true
                },
                          new ImageGallery
                {
                    Description = string.Empty,
                    Image = new UrlField { Description = "Image", Url = "/Style%20Library/Images/03.jpg"},
                    UrlNew = new UrlField { Description = "New", Url = "/Style%20Library/Images/03.jpg" },
                    OpenWindows = true,
                    Visible = true
                },
                             new ImageGallery
                {
                    Description = string.Empty,
                    Image = new UrlField { Description = "Image", Url = "/Style%20Library/Images/04.jpg"},
                    UrlNew = new UrlField { Description = "New", Url = "/Style%20Library/Images/04.jpg" },
                    OpenWindows = true,
                    Visible = true
                },
                                new ImageGallery
                {
                    Description = string.Empty,
                    Image = new UrlField { Description = "Image", Url = "/Style%20Library/Images/05.jpg"},
                    UrlNew = new UrlField { Description = "New", Url = "/Style%20Library/Images/05.jpg" },
                    OpenWindows = true,
                    Visible = true
                },
                                   new ImageGallery
                {
                    Description = string.Empty,
                    Image = new UrlField { Description = "Image", Url = "/Style%20Library/Images/06.jpg"},
                    UrlNew = new UrlField { Description = "New", Url = "/Style%20Library/Images/06.jpg" },
                    OpenWindows = true,
                    Visible = true
                },
                                      new ImageGallery
                {
                    Description = string.Empty,
                    Image = new UrlField { Description = "Image", Url = "/Style%20Library/Images/07.jpg"},
                    UrlNew = new UrlField { Description = "New", Url = "/Style%20Library/Images/07.jpg" },
                    OpenWindows = true,
                    Visible = true
                },   new ImageGallery
                {
                    Description = string.Empty,
                    Image = new UrlField { Description = "Image", Url = "/Style%20Library/Images/08.jpg"},
                    UrlNew = new UrlField { Description = "New", Url = "/Style%20Library/Images/08.jpg" },
                    OpenWindows = true,
                    Visible = true
                },
                                         new ImageGallery
                {
                    Description = string.Empty,
                    Image = new UrlField { Description = "Image", Url = "/Style%20Library/Images/09.jpg"},
                    UrlNew = new UrlField { Description = "New", Url = "/Style%20Library/Images/09.jpg" },
                    OpenWindows = true,
                    Visible = true
                },
                                         new ImageGallery
                {
                    Description = string.Empty,
                    Image = new UrlField { Description = "Image", Url = "/Style%20Library/Images/10.jpg"},
                    UrlNew = new UrlField { Description = "New", Url = "/Style%20Library/Images/10.jpg" },
                    OpenWindows = true,
                    Visible = true
                }
                                         ,
                                         new ImageGallery
                {
                    Description = string.Empty,
                    Image = new UrlField { Description = "Image", Url = "/Style%20Library/Images/11.jpg"},
                    UrlNew = new UrlField { Description = "New", Url = "/Style%20Library/Images/11.jpg" },
                    OpenWindows = true,
                    Visible = true
                }
                                         ,
                                         new ImageGallery
                {
                    Description = string.Empty,
                    Image = new UrlField { Description = "Image", Url = "/Style%20Library/Images/12.jpg"},
                    UrlNew = new UrlField { Description = "New", Url = "/Style%20Library/Images/12.jpg" },
                    OpenWindows = true,
                    Visible = true
                },
                                         new ImageGallery
                {
                    Description = string.Empty,
                    Image = new UrlField { Description = "Image", Url = "/Style%20Library/Images/09.jpg"},
                    UrlNew = new UrlField { Description = "New", Url = "/Style%20Library/Images/09.jpg" },
                    OpenWindows = true,
                    Visible = true
                }

                };
                foreach (var element in collection)
                {
                    repository.Insert(element);
                }


            }
        }


    }
}
