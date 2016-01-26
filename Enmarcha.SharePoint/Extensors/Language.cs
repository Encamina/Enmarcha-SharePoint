using System;
using System.Globalization;
using System.Linq;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Class.Logs;
using Microsoft.SharePoint;

namespace Enmarcha.SharePoint.Extensors
{
    public static class Language
    {
        private static readonly ILog Logger = new LogManager().GetLogger(new System.Diagnostics.StackTrace().GetFrame(0));
        /// <summary>
        /// Languaje function to activate some of which we have installed 
        /// </summary>
        /// <param name="web">Sitio Web donde lo queremos desplegar </param>
        /// <param name="lcidLanguage">LCID of language we want to activate</param>
        /// <returns></returns>
        public static bool EnableSiteCollectionLanguage(this SPWeb web, int lcidLanguage)
        {
            var result = true;
            try
            {
                if (web.Site.GetWebTemplates(web.Language)[web.WebTemplate].SupportsMultilingualUI)
                {
                    web.IsMultilingual = true;

                    var installed = SPLanguageSettings.GetGlobalInstalledLanguages(15);
                    var supported = web.SupportedUICultures;
                    foreach (SPLanguage language in installed)
                    {
                        var culture = new CultureInfo(language.LCID);
                        if (!supported.Contains(culture) && language.LCID == lcidLanguage)
                        {
                            web.AddSupportedUICulture(culture);
                        }
                    }
                    web.Update();
                }

            }
            catch (Exception exception)
            {

                Logger.Error("Error EnableSiteCollectionLanguage", exception.Message);
                result = false;
            }
            return result;
        }
    }
}
