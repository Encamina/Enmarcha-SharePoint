using System;
using System.Linq;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Microsoft.SharePoint;

namespace Enmarcha.SharePoint.Entities.Artefacts
{
   public sealed class ContentType:IContentType
    {       
        #region Properties

        public string Name { get; set; }
        public string GroupName { get; set; }
        public string Parent { get; set; }
        public SPWeb Web { get; set; }
        public ILog Logger { get; set; }

        #endregion

        #region Constructor

       /// <summary>
       /// Constructor
       /// </summary>
       /// <param name="web"></param>
       /// <param name="logger"></param>
       public ContentType(SPWeb web,ILog logger)
        {
            Web = web;
            Logger = logger;
        }

        public ContentType(SPWeb web,ILog logger, string name, string groupName, string parent) : this(web,logger)
        {
            Name = name;
            GroupName = groupName;
            Parent = parent;
        }

        #endregion

        #region Interface
        /// <summary>
        /// Create Te
        /// </summary>
        /// <returns></returns>
       public bool Create()
       {
           return Create(string.Empty);
       }
        /// <summary>
        /// Create the Content Type
        /// </summary>
        /// <param name="id">GUID the ContentType</param>
        /// <returns></returns>
        public bool Create(string id)
        {
            try
            {
                SPContentTypeId idContentType;
                SPContentType contentType;
                if (!string.IsNullOrEmpty(id))
                {
                    idContentType = new SPContentTypeId(id);
                    contentType = Web.ContentTypes[idContentType];
                    if (contentType != null)
                    {
                        return true;
                    }
                    contentType = new SPContentType(idContentType, Web.ContentTypes, Name);
                    Web.ContentTypes.Add(contentType);
                }
                else
                {
                    var itemCType = Web.AvailableContentTypes[Parent];
                    var cType = new SPContentType(itemCType, Web.ContentTypes, Name)
                    {
                        Group = GroupName
                    };
                     contentType = Web.ContentTypes.Add(cType);
                    idContentType = contentType.Id;
                }


                Web.ContentTypes[idContentType].Group = GroupName;
                Web.Update();
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Create ContentType:", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Delete the ContentType
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            try
            {
                var cType = Web.ContentTypes[Name];
                cType.Delete();
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Delete ContentType", exception.Message));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks for this content type 
        /// </summary>
        /// <returns></returns>
        public bool Exist()
        {
            try
            {
                var contentTypeCollection = Web.ContentTypes;
                return contentTypeCollection.Cast<SPContentType>().Any(itemContentType => itemContentType.Name == Name);
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Exist ContentType:", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Add Column Site of Content Type
        /// </summary>
        /// <param name="name">name of column</param>
        /// <returns></returns>
        public bool AddColumn(string name)
        {
            try
            {
                
                var contentType = Web.ContentTypes[Name];
                var field = GetField(name, Web);
                var fieldLink = new SPFieldLink(field);
                if (contentType.FieldLinks[fieldLink.Id] == null)
                {
                    contentType.FieldLinks.Add(fieldLink);
                }
                contentType.Update(true);
                
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Add Column ContentType: ", exception.Message));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Remove the Column Site in ContentType
        /// </summary>
        /// <param name="name">name of column</param>
        /// <returns></returns>
        public bool RemoveColumn(string name)
        {
            var result = true;
            try
            {
                
                var contentType = Web.ContentTypes[Name];
                contentType.FieldLinks.Delete(name);
                contentType.Update(true);
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Remove Column ContentType: ", exception.Message));
                result = false;
            }
            
            return result;
        }

        #endregion

        #region Methods Privates
        /// <summary>
        /// Get the field
        /// </summary>
        /// <param name="name">Name the Field</param>
        /// <param name="web"></param>
        /// <returns></returns>
        private static SPField GetField(string name, SPWeb web)
        {
            return web.Fields.Cast<SPField>().FirstOrDefault(field => field.InternalName.Equals(name));
        }
        #endregion
    }
}
