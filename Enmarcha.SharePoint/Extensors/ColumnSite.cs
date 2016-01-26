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
    public static class ColumnSite
    {
        private static readonly ILog Logger = new LogManager().GetLogger(new System.Diagnostics.StackTrace().GetFrame(0));
        /// <summary>
        ///Create a column  site based on a class
        /// </summary>
        /// <param name="web"></param>
        /// <param name="groupTerm"></param>
        /// <param name="genericClass"></param>
        /// <returns></returns>
        public static IList<ConfigurationColumn> CreateColumnSite(this SPWeb web, string groupTerm, Type genericClass)
        {
            var result = new List<ConfigurationColumn>();
            try
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
                    var groupTermTaxonomy = string.Empty;
                    var defaultValue = string.Empty;
                    var addPrefix = true;
                    var formula = string.Empty;
                    var formatCurrency = 0;
                    var date = TypeDate.DateTime;

                    foreach (var enmarcha in prop.GetCustomAttributes(true).Select(attribute => attribute as EnmarchaAttribute))
                    {
                        fieldType = enmarcha.Type.ToString();
                        create = enmarcha.Create;
                        term = enmarcha.Term;
                        multiValue = enmarcha.MultiValue;
                        displayName = enmarcha.DisplayName;
                        hidden = enmarcha.Hidden;
                        required = enmarcha.Required;
                        formula = enmarcha.Formula;

                        try
                        {
                            groupTermTaxonomy = enmarcha.Group;
                            addPrefix = enmarcha.AddPrefeix;
                            defaultValue = enmarcha.ValueDefault;
                            date = enmarcha.Date;
                            formatCurrency = enmarcha.FormatCurrency;

                        }

                        catch (Exception exception)
                        {

                            Logger.Error(string.Concat("ENMARCHA Error :", exception.Message));
                        }
                        result.Add(new ConfigurationColumn { Name = prop.Name, Prefix = addPrefix });
                        try
                        {
                            if (enmarcha.Choice != null)
                            {
                                choice.AddRange(enmarcha.Choice);
                            }
                        }
                        catch (Exception exception)
                        {
                            Logger.Error(string.Concat("Error :", exception.Message));
                        }
                    }

                    if (create)
                    {

                        CreateColumnSite(new ParamsCreateSite
                        {
                            Web = web,
                            Name = prop.Name,
                            FieldType = fieldType,
                            Requiered = required,
                            DefaultValue = defaultValue,
                            Choice = choice,
                            Group = groupTerm,
                            Term = term,
                            MultiValue = multiValue,
                            DisplayName = displayName,
                            Hidden = hidden,
                            GroupTerm = groupTermTaxonomy,
                            AddPrefix = addPrefix,
                            Formula = formula,
                            FormatCurrency = formatCurrency,
                            Date = date,                            
                        });
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return result;
        }

        /// <summary>
        /// Create one column site
        /// </summary>
        /// <param name="args"></param>
        private static void CreateColumnSite(ParamsCreateSite args)
        {
            try
            {                
                var type = GetFieldType(args.FieldType);
                if (args.FieldType.Equals("Taxonomy"))
                {
                    CreateColumnSiteTaxonomy(args, type);
                }
                else
                {
                    SiteColumn columnSite;
                    switch (type)
                    {
                        case SPFieldType.MultiChoice:
                        case SPFieldType.Choice:
                            columnSite = new SiteColumn(
                                new ParamsSiteColumnChoices
                                {
                                    Web = args.Web,
                                    Name = args.Name,
                                    FieldType = type,
                                    Choices = args.Choice,
                                    Group = args.Group,
                                    AddPrefix = args.AddPrefix,
                                    DisplayName = args.DisplayName
                                }, args.DefaultValue, args.Requiered);
                            break;
                        case SPFieldType.Calculated:
                            columnSite = new SiteColumn(
                                new ParamsSiteColumnBaseExtended
                                {
                                    Web = args.Web,
                                    Name = args.Name,
                                    FieldType = type,
                                    Requiered = args.Requiered,
                                    Group = args.Group,
                                    AddPrefix = args.AddPrefix,
                                    DisplayName = args.DisplayName
                                }, args.Formula);
                            break;
                        case SPFieldType.Currency:
                            columnSite = new SiteColumn(
                                new ParamsSiteColumnBaseExtended
                            {
                                Web = args.Web,
                                Name = args.Name,
                                FieldType = type,
                                Requiered = args.Requiered,
                                Group = args.Group,
                                AddPrefix = args.AddPrefix,
                                DisplayName = args.DisplayName
                            }, args.FormatCurrency);
                            break;
                        case SPFieldType.DateTime:
                            columnSite = new SiteColumn(
                                new ParamsSiteColumnBaseExtended
                            {
                                Web = args.Web,
                                Name = args.Name,
                                FieldType = type,
                                Requiered = args.Requiered,
                                Group = args.Group,
                                AddPrefix = args.AddPrefix,
                                DisplayName = args.DisplayName
                            }, args.Date);
                            break;
                        default:
                            columnSite = new SiteColumn(
                                new ParamsSiteColumnDefaultValue
                                {
                                Web = args.Web,
                                Name = args.Name,
                                FieldType = type,
                                Requiered = args.Requiered,
                                Group = args.Group,
                                AddPrefix = args.AddPrefix,
                                DefaultValue = args.DefaultValue,
                                MultiValue = args.MultiValue,
                                DisplayName = args.DisplayName
                            });

                            break;
                    }
                    CreateColumnSetParameters(args, columnSite);
                }
                args.Web.Update();                
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error CreateColumnSite", exception.Message));                
            }
        }

        private static bool CreateColumnSetParameters(ParamsCreateSite args, SiteColumn columnSite)
        {
            var result = false;
            if (!columnSite.Exist())
            {
                result = columnSite.Create();               
                if (args.Hidden) columnSite.Hidden();
            }
            return result;
        }
        /// <summary>
        /// Create one column site of Taxonomy
        /// </summary>
        /// <param name="args"></param>
        private static bool CreateColumnSiteTaxonomy(ParamsCreateSite args,  SPFieldType type)
        {
            var result = false;
            var columnSite =
                new SiteColumn(new ParamsSiteColumnDefaultValue
                {
                    Web = args.Web,
                    Name = args.Name,
                    FieldType = type,
                    Requiered = args.Requiered,
                    Group = args.Group,
                    AddPrefix = args.AddPrefix,
                    DefaultValue = args.DefaultValue
                });
            if (!columnSite.Exist())
            {
                result = columnSite.CreateTaxonomy(args.GroupTerm, args.Term, args.MultiValue, args.Requiered);
            }
            args.Name = (args.AddPrefix) ? string.Concat(Constants.Prefix, args.Name) : args.Name;
            columnSite.RenameField(args.Name,
                string.IsNullOrEmpty(args.DisplayName) 
                ? args.Name 
                : args.DisplayName);
            if (args.Hidden)
            {
                columnSite.Hidden();
            }
            return result;
        }

        #region Functions Privadas 

        internal static SPFieldType GetFieldType(string fieldType)
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
