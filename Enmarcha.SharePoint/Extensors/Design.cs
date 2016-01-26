using System;
using System.Collections.Generic;
using System.Linq;
using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Class.Logs;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing;
using Microsoft.SharePoint.WebPartPages;

namespace Enmarcha.SharePoint.Extensors
{
    public static class Design
    {
        private static readonly ILog Logger = new LogManager().GetLogger(new System.Diagnostics.StackTrace().GetFrame(0));
        /// <summary>
        /// Función para añadir JSLink a las librerias
        /// </summary>        
        /// <param name="list">Nombre de la lista</param>
        /// <param name="urlJS">Ubicación donde esta el fichero JS que se renderizara</param>
        /// <param name="typeForm">Typo de formulario donde se aplicara:Edit,New,Display,View</param>
        /// <returns></returns>
        public static bool AddJSLinkToLibrary(this SPList list, string urlJS, TypeForm typeForm)
        {
            try
            {
                var result = false;                
                var urlForm = GetTypeForm(list).Values.FirstOrDefault();
                var form = list.ParentWeb.GetFile(urlForm);
                var webPartManager =
                    form.GetLimitedWebPartManager(System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared);
                if (webPartManager.WebParts.Count > 0)
                {
                    if (TypeForm.View == typeForm)
                    {
                        var webPart = webPartManager.WebParts[0] as XsltListViewWebPart;
                        webPart.JSLink = urlJS;
                        webPartManager.SaveChanges(webPart);
                    }
                    else
                    {
                        var webPart = webPartManager.WebParts[0] as ListFormWebPart;
                        webPart.JSLink = urlJS;
                        webPartManager.SaveChanges(webPart);
                    }
                    result = true;
                }
                webPartManager.Web.Dispose();
                return result;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddJSLinkToLibrary", exception.Message));
                return false;
            }
        }


        private static Dictionary<TypeForm, string> GetTypeForm(SPList list)
        {
            return new Dictionary<TypeForm, string>
         {
             {TypeForm.New, list.DefaultNewFormUrl },
             {TypeForm.Edit, list.DefaultEditFormUrl },
             {TypeForm.Display, list.DefaultDisplayFormUrl  },
             {TypeForm.View, list.DefaultViewUrl }
         };
        }

        /// <summary>
        /// Funcion para Estabecer la MasterPage y la pagina de bienvenida por defecto
        /// </summary>
        /// <param name="web">Objeto SPWeb</param>
        /// <param name="customMasterUrl">Url del sitio donde esta la master  SPUrlUtility.CombineUrl(siteCollection.ServerRelativeUrl, "_catalogs/masterpage/mysite15ENCAMINA.master")</param>
        /// <param name="masterUrl"> Url done esta la master :  SPUrlUtility.CombineUrl(siteCollection.ServerRelativeUrl, "_catalogs/masterpage/mysite15ENCAMINA.master")</param>
        /// <param name="welcomePage">Nombre de la página de bienvenida Opcional</param>
        /// <param name="logoUrl">Url del logo Opcional</param>
        /// <returns></returns>
        public static bool SetMasterPage(SPWeb web, string customMasterUrl, string masterUrl)
        {
            return SetMasterPage(web, customMasterUrl, masterUrl, null);
        }

        /// <summary>
        /// Funcion para Estabecer la MasterPage y la pagina de bienvenida por defecto
        /// </summary>
        /// <param name="web">Objeto SPWeb</param>
        /// <param name="customMasterUrl">Url del sitio donde esta la master  SPUrlUtility.CombineUrl(siteCollection.ServerRelativeUrl, "_catalogs/masterpage/mysite15ENCAMINA.master")</param>
        /// <param name="masterUrl"> Url done esta la master :  SPUrlUtility.CombineUrl(siteCollection.ServerRelativeUrl, "_catalogs/masterpage/mysite15ENCAMINA.master")</param>
        /// <param name="welcomePage">Nombre de la página de bienvenida Opcional</param>
        /// <param name="logoUrl">Url del logo Opcional</param>
        /// <returns></returns>
        public static bool SetMasterPage(SPWeb web, string customMasterUrl, string masterUrl, string welcomePage)
        {
            return SetMasterPage(web, customMasterUrl, masterUrl, welcomePage, null);
        }

        /// <summary>
        /// Funcion para Estabecer la MasterPage y la pagina de bienvenida por defecto
        /// </summary>
        /// <param name="web">Objeto SPWeb</param>
        /// <param name="customMasterUrl">Url del sitio donde esta la master  SPUrlUtility.CombineUrl(siteCollection.ServerRelativeUrl, "_catalogs/masterpage/mysite15ENCAMINA.master")</param>
        /// <param name="masterUrl"> Url done esta la master :  SPUrlUtility.CombineUrl(siteCollection.ServerRelativeUrl, "_catalogs/masterpage/mysite15ENCAMINA.master")</param>
        /// <param name="welcomePage">Nombre de la página de bienvenida Opcional</param>
        /// <param name="logoUrl">Url del logo Opcional</param>
        /// <returns></returns>
        public static bool SetMasterPage(this SPWeb web, string customMasterUrl, string masterUrl, string welcomePage, string logoUrl)
        {
            var result = true;
            try
            {
                web.CustomMasterUrl = customMasterUrl;
                web.MasterUrl = masterUrl;
                web.Update();
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error SetMasterPage", exception.Message));
                result = false;
            }
            if (!string.IsNullOrEmpty(welcomePage))
            {
                SetWelcomePage(web, welcomePage);
            }
            if (!string.IsNullOrEmpty(logoUrl))
            {
                web.SiteLogoUrl = logoUrl;
            }
            web.Update();

            return result;
        }

        /// <summary>
        /// Establece la Página de bienveniva de un sitiio
        /// </summary>
        /// <param name="web">Objeto SPWeb </param>
        /// <param name="welcomePage"> Url de la página de bienvenida</param>
        /// <returns></returns>
        public static bool SetWelcomePage(this SPWeb web, string welcomePage)
        {
            var result = true;
            try
            {
                if (PublishingWeb.IsPublishingWeb(web))
                {
                    var publishingWeb = PublishingWeb.GetPublishingWeb(web);
                    var homeFile = web.GetFile(welcomePage);
                    publishingWeb.DefaultPage = homeFile;
                    publishingWeb.Update();
                }
                else
                {
                    web.RootFolder.WelcomePage = welcomePage;
                }
                web.Update();
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
        /// <summary>
        /// Función para aplicar temas
        /// ejemplo de variables que debemos de utilizar 
        /// var composedLookName = "Sareb";
        ///var paletteUrl = "_catalogs/theme/15/sareb.spcolor";
        ///var fontSchemeUrl = "_catalogs/theme/15/sarebfont.spfont";
        /// </summary>
        /// <param name="web">Objeto SPNmame</param>
        /// <param name="composedLookName"> Nombre del ComposedName</param>
        /// <param name="paletteUrl">Url donde esta la paleta</param>
        /// <param name="fontSchemeUrl"> Url donde esta la fuente</param>
        /// <returns></returns>
        public static bool ApplyTheme(this SPWeb web, string composedLookName, string paletteUrl, string fontSchemeUrl)
        {
            var result = true;
            try
            {
                var serverRelativeUrl = web.ServerRelativeUrl;
                if (!serverRelativeUrl.EndsWith("/"))
                {
                    serverRelativeUrl = string.Concat(serverRelativeUrl, "/");
                }
                var rootRealiveUrl = web.Site.RootWeb.ServerRelativeUrl;
                if (!rootRealiveUrl.EndsWith("/"))
                {
                    rootRealiveUrl = string.Concat(rootRealiveUrl, "/");
                }
                var masterPageUrl = web.MasterUrl;
                var list = web.GetCatalog(SPListTemplateType.DesignCatalog);
                var query = new SPQuery
                {
                    Query =
                        string.Format(
                            @"<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>{0}</Value></Eq></Where>",
                            composedLookName),
                    RowLimit = 10,
                    ViewAttributes = "Scope=\"Recursive\""
                };
                var found = list.GetItems(query);
                if (found.Count == 0)
                {
                    var item = list.AddItem();

                    item["Title"] = composedLookName;
                    item["Name"] = composedLookName;

                    var masterUrl = new SPFieldUrlValue
                    {
                        Url = string.Concat(serverRelativeUrl, masterPageUrl),
                        Description = string.Concat(serverRelativeUrl, masterPageUrl)
                    };
                    item["MasterPageUrl"] = masterUrl;

                    var themeUrl = new SPFieldUrlValue
                    {
                        Url = string.Concat(rootRealiveUrl, paletteUrl),
                        Description = string.Concat(rootRealiveUrl, paletteUrl)
                    };
                    item["ThemeUrl"] = themeUrl;

                    var imageUrl = new SPFieldUrlValue { Url = string.Empty, Description = string.Empty };
                    item["ImageUrl"] = imageUrl;

                    var fieldFontSchemeUrl = new SPFieldUrlValue { Url = fontSchemeUrl, Description = string.Empty };
                    item["FontSchemeUrl"] = string.Concat(rootRealiveUrl, fieldFontSchemeUrl);

                    item["DisplayOrder"] = 1;
                    item.Update();
                }
                web.ApplyTheme(string.Concat(rootRealiveUrl, paletteUrl), string.Concat(rootRealiveUrl, fontSchemeUrl),
                    null, true);
            }
            catch (Exception exception)
            {
                result = false;
                Logger.Error(string.Concat("Error ApplyTheme", exception.Message));
            }
            return result;
        }
    }
}
