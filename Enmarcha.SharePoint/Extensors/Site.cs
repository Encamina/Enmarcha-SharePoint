using System;
using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Class.Logs;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Applications.GroupBoard.MobileControls;

namespace Enmarcha.SharePoint.Extensors
{
    public static class Site
    {
        private static readonly ILog Logger = new LogManager().GetLogger(new System.Diagnostics.StackTrace().GetFrame(0));

        /// <summary>
        /// Create SubSite in SharePoint
        /// </summary>
        /// <param name="site"></param>
        /// <param name="urlWeb">Url para la Web que vamos a crear </param>
        /// <param name="title"></param>
        /// <param name="description"></param>        
        /// <param name="template">Nombre de la definición de sitios por ejemplo Team Template el valor que se debe pasar es "STS" </param>
        /// <returns></returns>
        public static bool CreateSite(this SPSite site, string urlWeb, string title, string description, string template)
        {
            return CreateSite(site, urlWeb, title, description, template, false);
        }

        /// <summary>
        /// Create SubSite in SharePoint
        /// </summary>
        /// <param name="site"></param>
        /// <param name="urlWeb">Url para la Web que vamos a crear </param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="breakPermission"></param>
        /// <param name="template">Nombre de la definición de sitios por ejemplo Team Template el valor que se debe pasar es "STS" </param>
        /// <returns></returns>
        public static bool CreateSite(this SPSite site, string urlWeb, string title, string description, string template, bool breakPermission)
        {
            try
            {
                var spWeb = site.RootWeb;
                var templateValue = string.IsNullOrEmpty(template) ? SPWebTemplate.WebTemplateSTS : template;
                var encSite = new Entities.Artefacts.Site(spWeb, Logger);
                return encSite.CreateSite(urlWeb, title, description, templateValue,
                    spWeb.Language, breakPermission);
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Create Site: ", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Create SubSite in SharePoint
        /// </summary>        
        /// <param name="web"></param>
        /// <param name="urlWeb">Url para la Web que vamos a crear </param>
        /// <param name="title"></param>
        /// <param name="description"></param>        
        /// <param name="template">Nombre de la definición de sitios por ejemplo Team Template el valor que se debe pasar es "STS"</param>
        /// <returns></returns>
        public static bool CreateSubSite(this SPWeb web, string urlWeb, string title, string description, string template)
        {
            return CreateSubSite(web, urlWeb, title, description, template, false);
        }

        /// <summary>
        /// Create SubSite in SharePoint
        /// </summary>        
        /// <param name="web"></param>
        /// <param name="urlWeb">Url para la Web que vamos a crear </param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="breakPermisions"></param>
        /// <param name="template">Nombre de la definición de sitios por ejemplo Team Template el valor que se debe pasar es "STS"</param>
        /// <returns></returns>
        public static bool CreateSubSite(this SPWeb web, string urlWeb, string title, string description, string template, bool breakPermisions)
        {
            try
            {
                var encSite = new Entities.Artefacts.Site(web, Logger);
                var templateValue = string.IsNullOrEmpty(template) ? SPWebTemplate.WebTemplateSTS : template;
                return encSite.CreateSite(urlWeb, title, description, templateValue,
                    web.Language, breakPermisions);

            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Create SubSite: ", exception.Message));
                return false;
            }
        }


        /// <summary>
        /// Function to add permissions to a site
        /// </summary>
        /// <param name="web"></param>
        /// <param name="group"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool AddPermisionSite(this SPWeb web, string group, RoleType role)
        {
            try
            {
                var encSite = new Entities.Artefacts.Site(web,Logger);
                return encSite.AddPermision(group, role);
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Create Site: ", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Remove Permision Library
        /// </summary>
        /// <param name="web"></param>
        /// <param name="group">Name Group</param>        
        /// <returns></returns>
        public static bool RemovePermisionSite(this SPWeb web, string group)
        {
            try
            {
                var encSite = new Entities.Artefacts.Site(web, Logger);
                return encSite.RemovePermision(group);
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Create Site: ", exception.Message));
                return false;
            }
        }
        /// <summary>
        /// Get the PropertyBag of SPWeb 
        /// </summary>
        /// <param name="web"></param>
        /// <param name="propertyName">Name of Property</param>
        public static string GetPropertyBag(this SPWeb web, string propertyName)
        {
            return web.AllProperties.ContainsKey(propertyName) ? web.AllProperties[propertyName].ToString() : string.Empty;
        }

        /// <summary>
        /// Set the PropertyBag of SPWeb 
        /// </summary>
        /// <param name="web"></param>
        /// <param name="propertyName">Name of Property</param>
        /// <param name="propertyValue">Value of Proerty</param>
        public static bool SetPropertyBag(this SPWeb web, string propertyName,string propertyValue)
        {
            try
            {
                if (!web.AllProperties.ContainsKey(propertyName))
                {
                    web.Properties.Add(propertyName, propertyValue);
                }
                else
                {
                    web.AllProperties[propertyName] = propertyValue;
                }
                web.Properties.Update();
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error SetPropertyBag: ",exception.Message) );
                return false;
            }
        }
    }
}
