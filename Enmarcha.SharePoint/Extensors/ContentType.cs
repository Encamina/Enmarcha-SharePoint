using System;
using System.Collections.Generic;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Class.Logs;
using Enmarcha.SharePoint.Helpers;
using Enmarcha.SharePoint.Helpers.Extensors;
using Microsoft.SharePoint;

namespace Enmarcha.SharePoint.Extensors
{
    public static class ContentType
    {
        private static readonly ILog Logger = new LogManager().GetLogger(new System.Diagnostics.StackTrace().GetFrame(0));
        /// <summary>
        /// Create Content Type and add a List of Site Column
        /// </summary>
        /// <param name="web"></param>
        /// <param name="name"></param>
        /// <param name="groupName"></param>
        /// <param name="parent"></param>
        /// <param name="columnSite"></param>
        /// <returns></returns>
        public static bool CreateContentType(this SPWeb web, string name, string groupName, string parent, IList<ConfigurationColumn> columnSite)
        {
            return CreateContentType(web, name, groupName, parent, columnSite, string.Empty);
        }
        /// <summary>
        /// Create a content 
        /// </summary>        
        /// <param name="web"></param>
        /// <param name="name"></param>
        /// <param name="groupName"></param>
        /// <param name="parent"></param>
        /// <param name="columnSite"></param>
        /// <param name="guidContentType"></param>
        /// <returns></returns>
        public static bool CreateContentType(this SPWeb web, string name, string groupName, string parent, IList<ConfigurationColumn> columnSite,
          string guidContentType)
        {
            try
            {
                var result = false;
                var contentType = new Entities.Artefacts.ContentType(web, Logger, name, groupName, parent);
                contentType.Create(guidContentType);
                foreach (var item in columnSite)
                {                    
                    result =
                        contentType.AddColumn( (!item.Prefix)?item.Name:string.Concat(Constants.Prefix, item.Name));

                }

                return result;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error CreateContentType", exception.Message));
                return false;
            }
        }
    }
}
