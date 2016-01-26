using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Attribute;
using Enmarcha.SharePoint.Class.Logs;
using Enmarcha.SharePoint.Entities.Artefacts;
using Enmarcha.SharePoint.Helpers;
using Enmarcha.SharePoint.Helpers.Extensors;
using Enmarcha.SharePoint.Helpers.SiteColumn;
using Microsoft.SharePoint;

namespace Enmarcha.SharePoint.Extensors
{
    public static class List
    {
        private static readonly ILog Logger = new LogManager().GetLogger(new System.Diagnostics.StackTrace().GetFrame(0));

        /// <summary>
        /// Create List
        /// </summary>
        /// <param name="web"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="typeList"></param>
        /// <param name="versionControl"></param>
        /// <returns></returns>
        public static bool CreateList(this SPWeb web, string title, string description, TypeList typeList,
            bool versionControl)
        {
            return CreateList(web, title, description, typeList, versionControl, null);
        }
        /// <summary>
        /// Create List
        /// </summary>        
        /// <param name="web"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="typeList"></param>
        /// <param name="versionControl"></param>
        /// <param name="genericClass"></param>
        /// <returns></returns>
        public static bool CreateList(this SPWeb web, string title, string description, TypeList typeList,
            bool versionControl, Type genericClass)
        {
            try
            {
                var list = new ListSharePoint(web, Logger, title);
                ListTemplateType type;
                switch (typeList)
                {
                    case TypeList.DocumentLibrary:
                        type = ListTemplateType.DocumentLibrary;
                        break;
                    case TypeList.PictureLibrary:
                        type = ListTemplateType.PictureLibrary;
                        break;
                    case TypeList.CalendarList:
                        type = ListTemplateType.Events;
                        break;
                    default:
                        type = ListTemplateType.GenericList;
                        break;
                }
                var result = list.Create(description, type, versionControl);
                if (genericClass != null) AddFieldInList(list, genericClass);

                return result;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Create List:", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Add permissions to the library
        /// </summary>        
        /// <param name="list"></param>
        /// <param name="group"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public static bool AddPermisionLibrary(this SPList list, string group, RoleType role)
        {
            try
            {

                var listSharePoint = new ListSharePoint(list.ParentWeb, Logger, list.Title);
                return listSharePoint.AddPermissionsGroup(group, role);
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddPermisionLibrary:", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Add ContentType in Library
        /// </summary>
        /// <param name="list"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static bool AddContentTypeLibrary(this SPList list, string contentType)
        {
            try
            {
                var listSharePoint = new ListSharePoint(list.ParentWeb,Logger, list.Title);
                if (listSharePoint.ExistContentType("Item")) listSharePoint.DeleteContentType("Item");
                if (listSharePoint.ExistContentType("Document")) listSharePoint.DeleteContentType("Document");
                if (listSharePoint.ExistContentType("Element")) listSharePoint.DeleteContentType("Element");
                return !listSharePoint.ExistContentType(contentType) && listSharePoint.AddContentType(contentType);
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddContentTypeLibrary", exception.Message));
                return false;
            }
        }


        #region Functions Privadas
        /// <summary>
        /// Add Field in List
        /// </summary>
        /// <param name="list"></param>
        /// <param name="genericClass"></param>
        private static void AddFieldInList(ListSharePoint list, Type genericClass)
        {
            var props = genericClass.GetProperties();
            foreach (var prop in props)
            {
                var fieldType = "Text";
                var create = true;
                var choice = new StringCollection();
                var term = string.Empty;
                var displayName = string.Empty;
                var hidden = false;
                var multiValue = false;
                var required = false;
                var group = Constants.Prefix;
                var defaultValue = string.Empty;
                var addPrefix = true;

                foreach (
                    var enmarcha in
                        prop.GetCustomAttributes(true).Select(attribute => attribute as EnmarchaAttribute))
                {
                    fieldType = enmarcha.Type.ToString();
                    create = enmarcha.Create;
                    term = enmarcha.Term;
                    multiValue = enmarcha.MultiValue;
                    displayName = enmarcha.DisplayName;
                    hidden = enmarcha.Hidden;
                    required = enmarcha.Required;
                    try
                    {
                        group = enmarcha.Group;
                        addPrefix = enmarcha.AddPrefeix;
                        defaultValue = enmarcha.ValueDefault;

                    }

                    catch (Exception exception)
                    {

                        Logger.Error(string.Concat("ENMARCHA Error :", exception.Message));
                    }

                    try
                    {
                        if (enmarcha.Choice != null) choice.AddRange(enmarcha.Choice);
                    }
                    catch (Exception exception)
                    {
                        Logger.Error(string.Concat("Error :", exception.Message));
                    }
                }

                if (!create) continue;
                CreateColumnInList(new ParamsColumnSite
                {
                    List = list,
                    Name = prop.Name,
                    FieldType = fieldType,
                    Requiered = required,
                    DefaultValue = defaultValue,
                    Choice = choice,
                    Group = group,
                    MultiValue = multiValue,
                    AddPrefix = addPrefix
                });
            }
        }

        private static void CreateColumnInList(ParamsColumnSite args)
        {
            var type = GetFieldType(args.FieldType);
            if (args.FieldType.Equals("Taxonomy"))
            {
                var columnSite = new SiteColumn
                    (new ParamsSiteColumnDefaultValue
                    {
                        Name = args.Name,
                        FieldType = type,
                        Requiered = args.Requiered,
                        Group = args.Group,
                        DefaultValue = args.DefaultValue,
                        AddPrefix = args.AddPrefix
                    });
                args.List.AddField(columnSite);

            }
            else
            {
                SiteColumn columnSite;
                switch (type)
                {
                    case SPFieldType.MultiChoice:
                    case SPFieldType.Choice:
                        columnSite = new SiteColumn(new ParamsSiteColumnChoices
                        {
                            Web = args.List.Web,
                            Name = args.Name,
                            FieldType = type,
                            Choices = args.Choice,
                            Group = args.Group,
                            AddPrefix = args.AddPrefix
                        },
                           args.DefaultValue, args.Requiered);
                        break;
                    default:
                        columnSite = new SiteColumn(new ParamsSiteColumnDefaultValue
                        {
                            Web = args.List.Web,
                            Name = args.Name,
                            FieldType = type,
                            Requiered = args.Requiered,
                            Group = args.Group,
                            DefaultValue = args.DefaultValue,
                            MultiValue = args.MultiValue,
                            AddPrefix = args.AddPrefix
                        });
                        break;
                }

                if (!args.List.ExistField(columnSite))
                {
                    args.List.AddField(columnSite);
                }


            }
        }


        private static SPFieldType GetFieldType(string fieldType)
        {
            
            SPFieldType result;
            GenerateCommand().TryGetValue(fieldType, out result);
            if (result == SPFieldType.Invalid) result = SPFieldType.Text;            
            return result;
        }

        private static Dictionary<string, SPFieldType> GenerateCommand()
        {
            return new Dictionary<string, SPFieldType>
            {
                {Constants.FieldTypeColumns.Boolean,SPFieldType.Boolean},
                {Constants.FieldTypeColumns.Choice,SPFieldType.Choice},
                {Constants.FieldTypeColumns.MultiChoice,SPFieldType.MultiChoice},
                {Constants.FieldTypeColumns.DateTime,SPFieldType.DateTime},
                {Constants.FieldTypeColumns.Lookup,SPFieldType.User},
                {Constants.FieldTypeColumns.Note,SPFieldType.Note},
                {Constants.FieldTypeColumns.Number,SPFieldType.Number},
                {Constants.FieldTypeColumns.Currency,SPFieldType.Currency},
                {Constants.FieldTypeColumns.Url,SPFieldType.URL},
                {Constants.FieldTypeColumns.Calculated,SPFieldType.Calculated},
            };

        }
        #endregion
    }
}
