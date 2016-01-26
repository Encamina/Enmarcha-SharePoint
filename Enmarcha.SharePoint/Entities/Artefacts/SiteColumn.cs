using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Helpers;
using Enmarcha.SharePoint.Helpers.SiteColumn;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace Enmarcha.SharePoint.Entities.Artefacts
{
    public sealed class SiteColumn:ISiteColumn
    {
    

        #region Properties

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public SPFieldType Type { get; set; }
        public bool Required { get; set; }
        public bool MultiValue { get; set; }
        public string DefaultValue { get; set; }
        public StringCollection Choice { get; set; }
        public string TargetListInternalName { get; set; }
        public string TargetListFieldInternalName { get; set; }
        public string GroupName { get; set; }
        public string Formula { get; set; }
        public IList<string> FieldAdditional { get; set; }

        public TypeDate Date { get; set; }
        public int Currency { get; set; }
        public SPWeb Web { get; set; }

        public  ILog Logger { get; set; }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor por defecto
        /// </summary>
        /// <param name="web"></param>
        /// <param name="logger"></param>
        public SiteColumn(SPWeb web,ILog logger)
        {
            Web = web;
            Logger = logger;
        }

        public SiteColumn(ParamsSiteColumnBase args) : this(args.Web,args.Logger)
        {
            Name = (args.AddPrefix) ? string.Concat(Constants.Prefix, args.Name) : args.Name;
            DisplayName = args.DisplayName;
        }


        /// <summary>
        ///  Constructor Columna por defecto
        /// </summary>
        /// <param name="args"></param>
        public SiteColumn(ParamsSiteColumnBaseExtended args) : this(args.Web,args.Logger)
        {

            Name = (args.AddPrefix) ? string.Concat(Constants.Prefix, args.Name) : args.Name;
            Type = args.FieldType;
            Required = args.Requiered;
            GroupName = args.Group;
            MultiValue = args.MultiValue;
            DisplayName = args.DisplayName;
        }

        /// <summary>
        /// Constructur Columna Currency
        /// </summary>
        /// <param name="args"></param>
        /// <param name="currency"></param>
        public SiteColumn(ParamsSiteColumnBaseExtended args, int currency) :this(args)            
        {
            Currency = currency;
        }

        /// <summary>
        /// Constructor Fecha
        /// </summary>
        /// <param name="args"></param>
        /// <param name="date"></param>
        public SiteColumn(ParamsSiteColumnBaseExtended args, TypeDate date)
            : this(args)
        {
            Date = date;
        }

        /// <summary>
        ///  Constructor Columna Calculada
        /// </summary>
        /// <param name="args"></param>
        /// <param name="formula"></param>
        public SiteColumn(ParamsSiteColumnBaseExtended args, string formula):
            this(args)
        {
            Formula = formula;
        }

        /// <summary>
        ///  Constructor Columna por defecto
        /// </summary>
        /// <param name="args"></param>
        public SiteColumn(ParamsSiteColumnDefaultValue args): this(args.Web,args.Logger)
         
        {
            Name = (args.AddPrefix) ? string.Concat(Constants.Prefix, args.Name) : args.Name;
            DisplayName = args.DisplayName;
            Type = args.FieldType;
            Required = args.Requiered;
            GroupName = args.Group;
            MultiValue = args.MultiValue;
            DefaultValue = args.DefaultValue;
        }


        /// <summary>
        /// Constructor Columna de Tipo Choice
        /// </summary>
        public SiteColumn(ParamsSiteColumnChoices args)
        {
            Web = args.Web;
            Name = (args.AddPrefix) ? string.Concat(Constants.Prefix, args.Name) : args.Name;
            DisplayName = args.DisplayName;
            Type = args.FieldType;
            Choice = args.Choices;
            GroupName = args.Group;
        }

        public SiteColumn(ParamsSiteColumnChoices args, string defaultValue)
            : this(args, defaultValue, false)
        {
        }

        public SiteColumn(ParamsSiteColumnChoices args, string defaultValue, bool required)
            : this(args)
        {
            DefaultValue = defaultValue;
            Required = required;
            DisplayName = args.DisplayName;
        }



        #endregion

        #region Interface

        /// <summary>
        /// Create Column Site
        /// </summary>
        /// <returns></returns>
        public bool Create()
        {
            try
            {
                switch (Type)
                {
                    case SPFieldType.Boolean:
                        CreateBoolean();
                        break;
                    case SPFieldType.Choice:
                    case SPFieldType.MultiChoice:
                        CreateChoice();
                        break;
                    case SPFieldType.DateTime:
                        CreateDateTime();
                        break;

                    case SPFieldType.User:

                        CreateUser();
                        break;
                    case SPFieldType.Calculated:
                        CreateCalculated();
                        break;
                    case SPFieldType.Currency:
                        CreateCurrency();
                        break;
                    default:
                        CreateDefault();
                        break;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("ColumnaSitio Create:", exception.Message));
                return false;
            }
            return true;
        }

        private void CreateDefault()
        {
            Web.Fields.Add(Name, Type, Required);
            if (!string.IsNullOrEmpty(GroupName))
            {
                var field = Web.Fields.GetFieldByInternalName(Name);
                field.Group = GroupName;
                field.Required = Required;
                field.Title = DisplayName;
                field.Update();    
            }
            
        }

        private void CreateCurrency()
        {
            Web.Fields.Add(Name, Type, Required);
            if (string.IsNullOrEmpty(GroupName))
            {
                return;
            }
            var field = (SPFieldCurrency)Web.Fields.GetFieldByInternalName(Name);

            field.Group = GroupName;
            field.Required = Required;
            field.CurrencyLocaleId = Currency;
            field.Title = DisplayName;
            field.Update();
        }

        private void CreateCalculated()
        {
            Web.Fields.Add(Name, Type, Required);
            var calculate = (SPFieldCalculated)Web.Fields.GetField(Name);
            calculate.Formula = Formula;
            if (!string.IsNullOrEmpty(GroupName))
            {
                calculate.Group = GroupName;
                calculate.Required = Required;
                calculate.Title = DisplayName;
                calculate.Update();
            }
            calculate.Update();
        }

        private void CreateUser()
        {
            Web.Fields.Add(Name, Type, Required);
            var user = (SPFieldUser)Web.Fields.GetField(Name);
            user.AllowMultipleValues = MultiValue;
            if (!string.IsNullOrEmpty(GroupName))
            {
                user.Group = GroupName;
                user.Required = Required;
                user.Title = DisplayName;
                user.Update();
            }
        }

        private void CreateDateTime()
        {
            var internalName = Web.Fields.Add(Name, Type, Required);
            if (!string.IsNullOrEmpty(GroupName))
            {
                var date = (SPFieldDateTime)Web.Fields.GetField(internalName);
                date.Group = GroupName;
                date.Required = Required;
                date.DisplayFormat = SPDateTimeFieldFormatType.DateTime;
                date.DefaultFormula = DefaultValue;
                date.Title = DisplayName;
                date.Update();
            }
        }

        private void CreateChoice()
        {
            if (Choice.Count > 0)
            {
                Web.Fields.Add(Name, Type, Required, false, Choice);
                var choice = Web.Fields.GetField(Name);
                if (!string.IsNullOrEmpty(GroupName))
                {
                    choice.Group = GroupName;
                }
                if (!string.IsNullOrEmpty(DefaultValue))
                {
                    choice.DefaultValue = DefaultValue;
                }
                choice.Required = Required;
                choice.Title = DisplayName;
                choice.Update();
            }
        }

        private void CreateBoolean()
        {
            Web.Fields.Add(Name, Type, Required);
            var boolean = (SPFieldBoolean)Web.Fields.GetField(Name);
            boolean.Group = GroupName;
            boolean.Required = Required;
            boolean.Title = DisplayName;
            if (!string.IsNullOrEmpty(DefaultValue))
            {
                boolean.DefaultValue = DefaultValue;

            }

            boolean.Update();
        }

        /// <summary>
        /// Create Column Taxonomy
        /// </summary>
        /// <param name="group"></param>
        /// <param name="termSet"></param>
        /// <param name="multiValue"></param>        
        /// <param name="requiered"></param>
        /// <returns></returns>
        public bool CreateTaxonomy(string group, string termSet, bool multiValue = false, bool requiered = false)
        {
            try
            {                
                var session = new TaxonomySession(Web.Site);
                var termStore = session.TermStores[0];
                var groupTx = termStore.Groups[group];
                var trmSet = groupTx.TermSets[termSet];

                var taxonomyField =
                (TaxonomyField)Web.Fields.CreateNewField("TaxonomyFieldType", Name);
                taxonomyField.SspId = termStore.Id;
                taxonomyField.Group = GroupName;
                taxonomyField.TermSetId = trmSet.Id;
                taxonomyField.Open = true;
                taxonomyField.AllowMultipleValues = multiValue;
                taxonomyField.Required = requiered;
                Web.Fields.Add(taxonomyField);
                Web.Update();             
                return true;
            }
            catch (Exception exception)
            {               
                Logger.Error(string.Concat("ColumnaSitio Create Taxonomy:", exception.Message));
                return false;
            }
        }
        /// <summary>
        /// Delete Column Site
        /// </summary>
        /// <returns></returns>
        public bool Delete()
        {
            try
            {
                var columnaDelete = Web.Fields.GetField(Name);
                columnaDelete.Delete();
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("ColumnaSitio Delete:", exception.Message));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Hidden Site Column
        /// </summary>
        /// <returns></returns>
        public bool Hidden()
        {
            try
            {
                var columnaHidden = Web.Fields.GetFieldByInternalName(Name);
                columnaHidden.Hidden = true;
                columnaHidden.Update();
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Hidden:", exception.Message));
                return false;
            }
            return true;
        }

        /// <summary>
        /// Check that there is a site column
        /// </summary>
        /// <returns></returns>
        public bool Exist()
        {
            try
            {
                var fieldCollection = Web.Fields;
                var result = fieldCollection.Cast<SPField>().Any(field => field.InternalName == Name);

                if (!result)
                {
                    fieldCollection = Web.Site.RootWeb.Fields;
                    result = fieldCollection.Cast<SPField>().Any(field => field.InternalName == Name);
                }
                return result;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("ColumnaSitio Exists", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Rename the site column
        /// </summary>
        /// <param name="oldName"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool RenameField(string oldName, string newName)
        {
            try
            {                
                var fieldAux = Web.Fields.GetField(oldName);
                fieldAux.StaticName = newName;
                fieldAux.Title = newName;
                fieldAux.Update(true);               
                return true;
            }
            catch (Exception exception)
            {
               Logger.Error(string.Concat("Error Rename Field:", exception.Message));
                return false;
            }
        }
        #endregion
    }
}
