using System;
using System.Collections.Generic;
using System.Linq;
using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;

namespace Enmarcha.SharePoint.Entities.Artefacts
{
    public sealed class ListSharePoint: IListSharePoint
    {
      
        #region Properties

        public string Name { get; set; }
        public SPWeb Web { get; set; }
        public ILog Logger { get; set; }
        #endregion

        #region Constructor

        public ListSharePoint(SPWeb web,ILog logger)
        {
            Web = web;
            Logger = logger;
        }

        public ListSharePoint(SPWeb web, ILog logger, string name) : this(web,logger)
        {
            Name = name;
        }

        #endregion

        #region Interface

        /// <summary>
        /// Create List
        /// </summary>
        /// <param name="description"></param>
        /// <param name="type"></param>        
        /// <returns></returns>
        public bool Create(string description, ListTemplateType type)
        {
            return Create(description,  type, false);
        }

        /// <summary>
        /// Create List
        /// </summary>
        /// <param name="description"></param>
        /// <param name="type"></param>
        /// <param name="versionControl"></param>
        /// <returns></returns>
        public bool Create(string description, ListTemplateType type, bool versionControl )
        {
            try
            {
                var guidList = Web.Lists.Add(Name, description, (SPListTemplateType) type);
                if (versionControl)
                {
                    var list = Web.Lists[guidList];
                    list.EnableVersioning = true;
                    list.Update();
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Delete List
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            try
            {
                Web.GetList(SPUrlUtility.CombineUrl(Web.Url, string.Concat("/lists/", Name))).Delete();
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Delete :", exception.Message));
                return false;
            }
            return true;
        }

        /// <summary>
        ///  This list exists or not?
        /// </summary>
        /// <returns></returns>
        public bool Exist()
        {
            try
            {
                var list = Web.GetList(SPUrlUtility.CombineUrl(Web.Url, string.Concat("/lists/", Name)));
                return (list == null);            
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Lista SP", exception.Message));
                return false;
            }          
        }

        /// <summary>
        /// Add Content Type
        /// </summary>
        /// <param name="contentTypeName"></param>
        /// <returns></returns>
        public bool AddContentType(string contentTypeName)
        {
            try
            {
                 var contentType = Web.Site.RootWeb.ContentTypes[contentTypeName] ?? Web.ContentTypes[contentTypeName];

                var listAdd = Web.Lists.TryGetList(Name);
                if (listAdd != null)
                {
                    listAdd.ContentTypesEnabled = true;
                    listAdd.Update();
                    listAdd.ContentTypes.Add(contentType);
                    listAdd.Update();
                    try
                    {
                        var orderedContentTypes = new SPContentType[1];
                        orderedContentTypes[0] = contentType;
                        listAdd.RootFolder.UniqueContentTypeOrder = orderedContentTypes;
                        listAdd.RootFolder.Update();
                    }
                    catch (Exception exception)
                    {
                     Logger.Error(string.Concat("Add ContentType", exception.Message));
                    }
                }
                else
                {
                    return false;
                }
                
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Add ContentType", exception.Message));                
                return false;
            }
            return true;
        }



        /// <summary>
        /// Delete ContentType
        /// </summary>
        /// <param name="contentTypeName"></param>
        /// <returns></returns>
        public bool DeleteContentType(string contentTypeName)
        {
            try
            {                
                var listAdd = Web.Lists.TryGetList(Name);
                listAdd.ContentTypes[contentTypeName].Delete();
                listAdd.Update();             
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Delete ContentType", exception.Message));               
                return false;
            }
            return true;
        }

        /// <summary>
        /// Existe este Tipo de contenido en la Lista
        /// </summary>
        /// <param name="contentTypeName"></param>
        /// <returns></returns>
        public bool ExistContentType(string contentTypeName)
        {
            try
            {
                var list = Web.Lists.TryGetList(Name);
                if (list != null)
                {
                    var listAdd = list.ContentTypes[contentTypeName];
                    return listAdd != null;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Add ContentType", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Añadir Campos
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool AddField(SiteColumn column)
        {
            try
            {
                var listEdit = Web.GetList(SPUrlUtility.CombineUrl(Web.Url, string.Concat("/lists/", Name)));
                switch (column.Type)
                {
                    case SPFieldType.Boolean:
                        AddFieldBoolean(column, listEdit);
                        break;
                    case SPFieldType.Choice:
                        AddFieldChoice(column, listEdit);
                        break;
                    case SPFieldType.DateTime:
                        AddFieldDateTime(column, listEdit);
                        break;
                    case SPFieldType.Lookup:
                        AddFieldLookup(column, listEdit);

                        break;
                    case SPFieldType.Calculated:
                        AddFieldCalculated(column, listEdit);
                        break;

                    case SPFieldType.Currency:
                        Web.Fields.Add(Name, column.Type, column.Required);
                        AddFieldCurrency(column);
                        break;
                    default:
                        var item= listEdit.Fields.Add(column.Name, column.Type, column.Required);
                        var fieldDefault = listEdit.Fields.GetField(item);
                        fieldDefault.Title = column.DisplayName;
                        fieldDefault.Update(true);
                        break;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Add Field", exception.Message));
                return false;
            }
            return true;
        }

       
      

        /// <summary>
        /// Delete Field
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool DeleteField(SiteColumn column)
        {
            var listEdit = Web.Lists.TryGetList(Name);
            if (listEdit != null)
            {
                listEdit.Fields.GetField(column.Name).Delete();
            }         
            return true;
        }

      
   
        /// <summary>
        /// Add permissions fror Group in SharePoint
        /// </summary>
        /// <param name="group"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool AddPermissionsGroup(string group, RoleType role)
        {
            try
            {             
                var listAdd = Web.Lists[Name];
                var oGroup = Web.Site.RootWeb.SiteGroups.GetByName(group);
                var roleAssignment = new SPRoleAssignment(oGroup);
                var roleDefinition = Web.RoleDefinitions.GetByType((SPRoleType) role);
                roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                if (!listAdd.HasUniqueRoleAssignments)
                {
                    listAdd.BreakRoleInheritance(false);
                }
                listAdd.RoleAssignments.Add(roleAssignment);
                listAdd.Update(true);             
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddPermission :", exception.Message));
                return false;
            }
        }
        /// <summary>
        /// Delete Permisions for Group
        /// </summary>
        /// <param name="group"></param>        
        /// <returns></returns>
        public bool RemovePermissionsGroup(string group)
        {
            try
            {                
                var listAdd = Web.Lists[Name];
                var oGroup = Web.Site.RootWeb.SiteGroups.GetByName(group);
                listAdd.RoleAssignments.RemoveById(oGroup.ID);
                listAdd.Update();             
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddPermission :", exception.Message));
                return false;
            }
        }
        /// <summary>
        /// Funcion que elimina los permisos en una lista
        /// </summary>
        /// <returns></returns>
        public bool ClearPermisions()
        {
            try
            {                
                var list = Web.Lists[Name];
                if (!list.HasUniqueRoleAssignments)
                {
                    list.BreakRoleInheritance(false);
                }
                var numRoles = list.RoleAssignments.Count;
                for (var i = 0; i < numRoles; i++)
                {
                    list.RoleAssignments.Remove(i);
                    list.Update();
                }             
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error ClearPermission :", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Create a folder in the document libraries
        /// </summary>
        /// <param name="name"/>Nombre de la Carpeta a crear ydonde va a estar la carpet: Example
        ///  "/Document Library/Folder1" 
        ////Creates a folder named “Folder11” under “Folder1” in “Document Library” in a sub site</param>        
        /// <returns></returns>
        public bool CreateFolder(string name)
        {
            try
            {

                var list = Web.Lists.TryGetList(Name);
                if (list == null)
                {
                    return false;
                }

                var folderCollection = list.RootFolder.SubFolders;

                folderCollection.Add(name);

                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Añadir Folder", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Da permisos a la carpeta especificada
        /// </summary>
        /// <param name="name">Ruta de la Carpeta</param>
        /// <param name="group">Grupo de Usuarios a los que da permisos</param>
        /// <param name="role">Rol de la administrador</param>
        /// <returns></returns>
        public bool AddPermissionsInFolder(string name, string group, SPRoleType role)
        {
            try
            {                
                var list = Web.Lists.TryGetList(Name);
                if (list == null)
                {
                    return false;
                }
                var folderCollection = list.RootFolder.SubFolders[name];
                if (!list.HasUniqueRoleAssignments)
                {
                    list.BreakRoleInheritance(false);
                }
                if (!folderCollection.Item.HasUniqueRoleAssignments)
                {
                    folderCollection.Item.BreakRoleInheritance(false);
                }
                var oGroup = Web.SiteGroups[group];
                var roleAssignment = new SPRoleAssignment(oGroup);
                var roleDefinition = Web.RoleDefinitions.GetByType(role);
                roleAssignment.RoleDefinitionBindings.Add(roleDefinition);

                folderCollection.Item.RoleAssignments.Add(roleAssignment);
                folderCollection.Item.Update();                

                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Exceptin Folder", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Return content type the sharepoint list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetContentType()
        {
            try
            {
                var contentTypeCollection = Web.Lists[Name].ContentTypes;

                return (from SPContentType contentType in contentTypeCollection select contentType.Name).ToList();
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error GetContentType :", exception.Message));
                return null;
            }
        }

        /// <summary>
        /// Existe esta Columna de Lista
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool ExistField(SiteColumn column)
        {
            try
            {
                var listEdit = Web.Lists.TryGetList(Name);
                var spfield = listEdit.Fields.GetField(column.Name);
                return spfield != null;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error:", exception.Message));
                return false;
            }
        }
        #endregion

        #region Methods Private
        private static void AddFieldCalculated(SiteColumn column, SPList listEdit)
        {
            listEdit.Fields.Add(column.Name, column.Type, column.Required);
            var calculate = (SPFieldCalculated)listEdit.Fields.GetField(column.Name);
            calculate.Formula = column.Formula;
            calculate.Title = column.DisplayName;
            calculate.Required = column.Required;

            calculate.Update();
        }

        private void AddFieldLookup(SiteColumn column, SPList listEdit)
        {
            var targetList =
                Web.GetList(SPUrlUtility.CombineUrl(Web.Url, string.Concat("/lists/", column.TargetListInternalName)));

            listEdit.Fields.AddLookup(column.Name, targetList.ID, column.Required);
            var lookup = (SPFieldLookup)listEdit.Fields.GetField(column.Name);
            lookup.LookupField =
                targetList.Fields[
                    targetList.Fields.GetFieldByInternalName(column.TargetListFieldInternalName).Id]
                    .InternalName;
            lookup.Update();
            foreach (var fieldAdditional in column.FieldAdditional)
            {
                var depLookUp = listEdit.Fields.AddDependentLookup(fieldAdditional, lookup.Id);
                var fieldDepLookup = (SPFieldLookup)listEdit.Fields.GetFieldByInternalName(depLookUp);
                fieldDepLookup.LookupWebId = Web.ParentWeb.ID;
                fieldDepLookup.LookupField =
                    targetList.Fields[targetList.Fields.GetFieldByInternalName(fieldAdditional).Id]
                        .InternalName;
                fieldDepLookup.Update();
            }
        }

        private static void AddFieldDateTime(SiteColumn column, SPList listEdit)
        {
            listEdit.Fields.Add(column.Name, column.Type, column.Required);
            var date = (SPFieldDateTime)listEdit.Fields.GetField(column.Name);
            date.DisplayFormat = column.Date == TypeDate.Date
                ? SPDateTimeFieldFormatType.DateOnly
                : SPDateTimeFieldFormatType.DateTime;
            date.Title = column.DisplayName;
            date.Update();
        }

        private static void AddFieldChoice(SiteColumn column, SPList listEdit)
        {
            if (column.Choice.Count <= 0)
            {
                return;
            }
            listEdit.Fields.Add(column.Name, column.Type, column.Required, false,
                column.Choice);
            if (string.IsNullOrEmpty(column.DefaultValue))
            {
                return;
            }
            var choice = (SPFieldChoice)listEdit.Fields.GetField(column.Name);
            choice.Title = column.DisplayName;
            choice.DefaultValue = column.DefaultValue;
            choice.Update();
        }

        private static void AddFieldBoolean(SiteColumn column, SPList listEdit)
        {
            listEdit.Fields.Add(column.Name, column.Type, column.Required);
            if (string.IsNullOrEmpty(column.DefaultValue))
            {
                return;
            }
            if (column.DefaultValue != "0" && column.DefaultValue != "1")
            {
                return;
            }
            var boolean = (SPFieldBoolean)listEdit.Fields.GetField(column.Name);
            boolean.DefaultValue = column.DefaultValue;
            boolean.Title = column.DisplayName;
            boolean.Update();
        }

        private void AddFieldCurrency(SiteColumn column)
        {
            if (string.IsNullOrEmpty(column.GroupName))
            {
                return;
            }
            var field = (SPFieldCurrency)Web.Fields.GetFieldByInternalName(Name);
            field.Title = column.DisplayName;
            field.Group = column.GroupName;
            field.Required = column.Required;
            field.CurrencyLocaleId = column.Currency;
            field.Update();
        }

        #endregion

    }
}
