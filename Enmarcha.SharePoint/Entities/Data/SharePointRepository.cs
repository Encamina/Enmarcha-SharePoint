using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Abstract.Interfaces.Data;
using Enmarcha.SharePoint.Attribute;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using Enmarcha.SharePoint.Helpers.Data;

namespace Enmarcha.SharePoint.Class.Data
{
    public sealed class SharePointRepository<T> : IRepository<T> where T : new()
    {
        #region Properties
        private ILog Logger { get; set; }
        private string Lista { get; set; }
        private int pageSize { get; set; }
        private SPWeb Web { get; set; }
        private const string Prefix = "ENMARCHA";
        private bool InternalName { get; set; }
        #endregion

        #region Constructor
        public SharePointRepository(SPWeb web, ILog logger, string lista, int pageSize)
            : this(web, logger, lista, pageSize, false)
        {
        }

        public SharePointRepository(SPWeb web, ILog logger, string lista, int pageSize, bool internalName)
        {
            Lista = lista;
            this.pageSize = pageSize;
            Web = web;
            InternalName = internalName;
            Logger = logger;
        }
        #endregion
        #region Interface
        public int PageSize()
        {
            return pageSize;
        }

        /// <summary>
        /// Insert the item in a SharePoint list
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int Insert(T data)
        {
            try
            {                
                var list = !InternalName ? Web.Lists[Lista] : Web.GetListFromWebPartPageUrl(Lista);
                var item = list.AddItem();
                var props = typeof(T).GetProperties();
                SetValueClass(data, props, item, Web);
                item.Update();
                return item.ID;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Insert ID", exception.HResult, " : ", exception.Message));
                return -1;
            }

        }
        /// <summary>
        /// Update the item in a List of the SharePoint
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Save(int id, T data)
        {
            try
            {                
                var list = !InternalName ? Web.Lists[Lista] : Web.GetListFromWebPartPageUrl(Lista);
                var listItem = list.GetItemById(Convert.ToInt32(id.ToString()));
                if (listItem == null)
                {
                    return true;
                }
                var props = typeof(T).GetProperties();

                SetValueClass(data, props, listItem, Web);

                listItem.Update();
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Save ID", id, exception.HResult, " : ", exception.Message));
                return false;
            }
        }
        /// <summary>
        /// Delete a item by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(int id)
        {
            try
            {                
                var list = !InternalName ? Web.Lists[Lista] : Web.GetListFromWebPartPageUrl(Lista);
                var listItem = list.GetItemById(Convert.ToInt32(id.ToString()));
                if (listItem != null)
                {
                    listItem.Delete();
                }
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Delete ID", id, exception.HResult, " : ", exception.Message));
                return false;
            }
        }
        /// <summary>
        /// Deletes an item and sends it to the Recycle Bin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteInRecicle(int id)
        {
            try
            {                
                var list = !InternalName ? Web.Lists[Lista] : Web.GetListFromWebPartPageUrl(Lista);
                var listItem = list.GetItemById(Convert.ToInt32(id.ToString()));
                if (listItem != null)
                {
                    listItem.Recycle();
                }
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Delete ID", id, exception.HResult, " : ", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Obtained by the indicating element
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T Get(int id)
        {
            try
            {
                T result;
                var list = !InternalName ? Web.Lists[Lista] : Web.GetListFromWebPartPageUrl(Lista);

                var listItem = list.GetItemById(Convert.ToInt32(id.ToString()));
                {
                    var valor = new T();
                    var props = typeof(T).GetProperties();
                    foreach (var prop in props)
                    {
                        GetValue(listItem, prop, valor);
                    }
                    result = valor;
                }
                return result;
            }
            catch (Exception)
            {
                return new T();
            }
        }

        #endregion

        #region Methods private
        private void GetValue(SPListItem listItem, PropertyInfo prop, T valor)
        {
            try
            {
                var multiple = false;
                var type = string.Empty;
                string nameInternal;
                bool existe;
                if (ExtractAttributtes(listItem, prop, ref multiple, ref type, out nameInternal, out existe)) return;
                if (!existe) return;
                Logger.Info(string.Concat("Prop Name", prop.Name));
                Logger.Info(string.Concat("Get Value Type", listItem[nameInternal].GetType().Name));
                switch (listItem[nameInternal].GetType().Name)
                {

                    case "DateTime":
                        SetDateTimeField(listItem, prop, valor, nameInternal);
                        break;
                    case "TaxonomyFieldValue":
                        SetTaxonomyField(listItem, prop, valor, nameInternal);
                        break;
                    case "TaxonomyFieldValueCollection":
                        SetTaxonomyFieldValueCollection(listItem, prop, valor, nameInternal);
                        break;
                    case "SPFieldCurrency":
                        SetCurrencyField(listItem, prop, valor, nameInternal);
                        break;

                    default:
                        SetDefault(listItem, prop, valor, nameInternal, type, multiple);
                        break;
                }
            }

            catch (Exception exception)
            {
                Logger.Error(exception.Message);
            }
        }

        private void SetDefault(SPListItem listItem, PropertyInfo prop, T valor, string nameInternal, string type, bool multiple)
        {
            try
            {
                var field = listItem.Fields.GetFieldByInternalName(nameInternal);
                var fieldValueType = field.FieldValueType.Name;
                if (fieldValueType.Contains("Boolean"))
                {
                    SetFieldBoolean(listItem, prop, valor, nameInternal);
                    return;
                }

                if (fieldValueType.Contains("Double"))
                {
                    SetFieldDouble(listItem, prop, valor, type, nameInternal);
                    return;
                }
                if (fieldValueType.Contains("Lookup"))
                {
                    SetFieldLookup(listItem, prop, valor, nameInternal);
                    return;
                }
                if (fieldValueType.Contains("Url"))
                {
                    SetFieldUrl(listItem, prop, valor, nameInternal);
                }
                if (fieldValueType.Contains("User"))
                {
                    SetFieldTypeUSer(listItem, prop, valor, nameInternal, multiple);
                }
                else
                {
                    SetFieldTypeDefault(listItem, prop, valor, nameInternal);
                }

            }
            catch (Exception exception)
            {
                Logger.Warn("Aviso" + exception.Message);
                prop.SetValue(valor,
                    (listItem[nameInternal] != null ? listItem[nameInternal].ToString() : ""));
            }
        }

        private static void SetFieldTypeDefault(SPListItem listItem, PropertyInfo prop, T valor, string nameInternal)
        {
            if (listItem[nameInternal].ToString().Contains(";#"))
            {
                SetFieldLookupString(listItem, prop, valor, nameInternal);
            }
            else
            {
                SetFieldDefault(listItem, prop, valor, nameInternal);
            }
        }

        private static void SetFieldTypeUSer(SPListItem listItem, PropertyInfo prop, T valor, string nameInternal, bool multiple)
        {
            var data = listItem[nameInternal].ToString();
            if (!multiple)
            {
                SetFieldUser(listItem, prop, valor, nameInternal, data);
            }
            else
            {
                SetFieldUserMultiple(listItem, prop, valor, nameInternal, data);
            }
        }

        private static void SetFieldDefault(SPListItem listItem, PropertyInfo prop, T valor, string nameInternal)
        {
            prop.SetValue(valor,
                (listItem[nameInternal] != null ? listItem[nameInternal].ToString() : ""));
        }

        private static void SetFieldLookupString(SPListItem listItem, PropertyInfo prop, T valor, string nameInternal)
        {
            IList<LookupFieldEnc> listLookup = new List<LookupFieldEnc>();
            listLookup.Add(new LookupFieldEnc
            {
                Key =
                    listItem[nameInternal].ToString()
                        .Split('#')
                        .GetValue(0)
                        .ToString()
                        .Replace(";", ""),
                Value = listItem[nameInternal].ToString()
                    .Split('#')
                    .GetValue(1)
                    .ToString()
                    .Replace(";", "")
            });
            prop.SetValue(valor, listLookup);
        }

        private static void SetFieldUserMultiple(SPListItem listItem, PropertyInfo prop, T valor, string nameInternal,
            string data)
        {
            var field = (SPFieldUser)listItem.Fields.GetFieldByInternalName(nameInternal);
            var user = (SPFieldUserValueCollection)field.GetFieldValue(data);
            IList<UserSP> listUser = new List<UserSP>();
            foreach (var userValue in user)
            {
                listUser.Add(
                    (userValue.User != null)
                        ? new UserSP
                        {
                            Key = userValue.User.ID.ToString(),
                            Value = userValue.User.Name,
                            LoginName = userValue.User.LoginName
                        }
                        : new UserSP
                        {
                            Key = userValue.LookupId.ToString(),
                            Value = userValue.LookupValue,
                            LoginName = userValue.LoginName
                        }
                    );
            }
            prop.SetValue(valor, listUser);
        }

        private static void SetFieldUser(SPListItem listItem, PropertyInfo prop, T valor, string nameInternal, string data)
        {
            var field = (SPFieldUser)listItem.Fields.GetFieldByInternalName(nameInternal);
            var user = (SPFieldUserValue)field.GetFieldValue(data);
            IList<UserSP> listUser = new List<UserSP>();

            listUser.Add(
                (user.User != null)
                    ? new UserSP
                    {
                        Key = user.User.ID.ToString(),
                        Value = user.User.Name,
                        LoginName = user.User.LoginName
                    }
                    : new UserSP
                    {
                        Key = user.LookupId.ToString(),
                        Value = user.LookupValue,
                        LoginName = user.LoginName
                    });
            prop.SetValue(valor, listUser);
        }

        private static void SetFieldUrl(SPListItem listItem, PropertyInfo prop, T valor, string nameInternal)
        {
            var data = listItem[nameInternal].ToString();
            var url = (SPFieldUrl)listItem.Fields.GetFieldByInternalName(nameInternal);
            var urlField = (SPFieldUrlValue)url.GetFieldValue(data);
            var urlList = new Helpers.Data.UrlField
            {
                Url = urlField.Url,
                Description = urlField.Description
            };
            prop.SetValue(valor, urlList);
        }

        private static void SetFieldLookup(SPListItem listItem, PropertyInfo prop, T valor, string nameInternal)
        {
            var collectionLookup = new SPFieldLookupValue(listItem[nameInternal].ToString());

            var lookup = new LookupFieldEnc
            {
                Key = collectionLookup.LookupId.ToString(),
                Value = collectionLookup.LookupValue
            };
            prop.SetValue(valor, lookup);
        }

        private static void SetFieldDouble(SPListItem listItem, PropertyInfo prop, T valor, string type, string nameInternal)
        {
            if (type == TypeField.Integer.ToString())
            {
                prop.SetValue(valor,
                    (listItem[nameInternal] != null ? Convert.ToInt32(listItem[nameInternal].ToString()) : 0));
            }
            else if (type == TypeField.Currency.ToString())
            {
                var monetaryValue = listItem[nameInternal] != null ? Convert.ToDouble(listItem[nameInternal].ToString()) : 0;
                var field = (SPFieldCurrency)listItem.Fields.GetFieldByInternalName(nameInternal);
                var format = field.CurrencyLocaleId;
                var currency = new Currency { Format = format, Value = monetaryValue };

                prop.SetValue(valor, currency);
            }
            else
            {
                prop.SetValue(valor,
                    (listItem[nameInternal] != null ? Convert.ToDouble(listItem[nameInternal].ToString()) : 0));
            }
        }

        private static void SetFieldBoolean(SPListItem listItem, PropertyInfo prop, T valor, string nameInternal)
        {
            if (listItem[nameInternal] == null)
            {
                prop.SetValue(valor, false);
            }
            else
            {
                var valorBoolean = (listItem[nameInternal].ToString() == "True");
                prop.SetValue(valor, valorBoolean);
            }
        }

        private static void SetCurrencyField(SPListItem listItem, PropertyInfo prop, T valor, string nameInternal)
        {
            var value = listItem[nameInternal] != null ? Convert.ToDouble(listItem[nameInternal].ToString()) : 0;
            var fieldCurrency = (SPFieldCurrency)listItem[nameInternal];
            var formato = fieldCurrency.CurrencyLocaleId;
            prop.SetValue(valor, new Currency { Value = value, Format = formato });
        }

        private static void SetTaxonomyFieldValueCollection(SPListItem listItem, PropertyInfo prop, T valor, string nameInternal)
        {
            var itemsTaxonomy = listItem[nameInternal].ToString().Split(';');
            IList<Taxonomy> listTaxonomy =
                itemsTaxonomy.Select(itemTax => itemTax.Split('|')).
                Select(taxonomy => new Taxonomy
                {
                    Key = taxonomy.GetValue(1).ToString(),
                    Value = taxonomy.GetValue(0).ToString()
                }).ToList();
            prop.SetValue(valor, listTaxonomy);
        }

        private static void SetTaxonomyField(SPListItem listItem, PropertyInfo prop, T valor, string nameInternal)
        {
            var collectionTaxonomy = (TaxonomyFieldValue)listItem[nameInternal];
            var taxonomyList = new List<Taxonomy>
            {
                new Taxonomy
                {
                    Key = collectionTaxonomy.TermGuid,
                    Value = collectionTaxonomy.Label
                }
            };
            prop.SetValue(valor, taxonomyList);
        }

        private static void SetDateTimeField(SPListItem listItem, PropertyInfo prop, T valor, string nameInternal)
        {
            var date = Convert.ToDateTime(listItem[nameInternal].ToString());
            prop.SetValue(valor, date);
        }

        private bool ExtractAttributtes(SPListItem listItem, PropertyInfo prop, ref bool multiple,
            ref string type, out string nameInternal, out bool existe)
        {
            var addPrefix = false;
            foreach (
                var enmarcha in
                    prop.GetCustomAttributes(true).Select(attribute => attribute as EnmarchaAttribute))
            {
                addPrefix = enmarcha.AddPrefeix;
                multiple = enmarcha.MultiValue;
                type = enmarcha.Type.ToString();
            }
            nameInternal = GetInternalName(prop.Name, addPrefix);
            existe = true;
            if (listItem[nameInternal] == null)
            {
                return true;
            }
            try
            {
                var existField = listItem.Fields.GetFieldByInternalName(nameInternal);
                if (existField == null)
                {
                    existe = false;
                }
            }
            catch (Exception exception)
            {
                existe = false;
                Logger.Error("Error" + exception.Message);
            }
            return false;
        }

        public ICollection<T> GetAll()
        {
            ICollection<T> result = new List<T>();
            var list = !InternalName ? Web.Lists[Lista] : Web.GetListFromWebPartPageUrl(Lista);
            var camlQuery = new SPQuery { ViewXml = "<View/>", RowLimit = 2000 };
            var listItems = list.GetItems(camlQuery);
            foreach (SPListItem listItem in listItems)
            {
                var valor = new T();
                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                {
                    GetValue(listItem, prop, valor);
                }
                result.Add(valor);
            }
            return result;
        }

        public ICollection<T> GetAll(int page)
        {
            ICollection<T> result = new List<T>();
            ICollection<T> resultReturn = new List<T>();
            var list = !InternalName ? Web.Lists[Lista] : Web.GetListFromWebPartPageUrl(Lista);
            var camlQuery = new SPQuery { ViewXml = "<View/>", RowLimit = Convert.ToUInt32(page * pageSize) };
            var listItems = list.GetItems(camlQuery);
            foreach (SPListItem listItem in listItems)
            {
                var valor = new T();
                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                {
                    GetValue(listItem, prop, valor);
                }
                result.Add(valor);
            }
            var resultPreview = result.ToList();
            var max = (result.Count < page * pageSize) ? result.Count : page * pageSize;
            for (var i = (page * pageSize) - pageSize; i < max; i++)
            {
                resultReturn.Add(resultPreview[i]);
            }
            return resultReturn;
        }

        public ICollection<T> Query(IQuery query, int page)
        {
            return ResultReturn(query.Execute(), page);
        }

        public ICollection<T> Query(string query, int page)
        {
            return ResultReturn(query, page);
        }

        private ICollection<T> ResultReturn(string query, int page)
        {
            ICollection<T> result = new List<T>();
            ICollection<T> resultReturn = new List<T>();
            var list = !InternalName ? Web.Lists[Lista] : Web.GetListFromWebPartPageUrl(Lista);
            var camlQuery = new SPQuery { Query = query, RowLimit = Convert.ToUInt32(page * pageSize) };
            var listItems = list.GetItems(camlQuery);
            foreach (SPListItem listItem in listItems)
            {
                var valor = new T();
                var props = typeof(T).GetProperties();
                foreach (var prop in props)
                {
                    GetValue(listItem, prop, valor);
                }
                result.Add(valor);
            }
            var resultPreview = result.ToList();
            var max = (result.Count < page * pageSize) ? result.Count : page * pageSize;
            for (var i = (page * pageSize) - pageSize; i < max; i++)
            {
                resultReturn.Add(resultPreview[i]);
            }
            return resultReturn;
        }


        private static string GetInternalName(string name, bool prefix)
        {
            return prefix ? string.Concat(Prefix, name) : name;
        }

        private void SetValueClass(T data, IEnumerable<PropertyInfo> props, SPListItem item, SPWeb web)
        {
            foreach (var prop in props)
            {
                object valor;
                string fieldType;
                bool addPrefix;
                var existe = ExtractAttributes(data, item, prop, out valor, out fieldType, out addPrefix);
                if (existe && (prop.Name != "ID"))
                {
                    SetTypeField(item, web, fieldType, valor, prop, addPrefix);
                }
            }
        }

        private void SetTypeField(SPListItem item, SPWeb web, string fieldType, object valor, PropertyInfo prop, bool addPrefix)
        {
            try
            {
                switch (fieldType)
                {
                    case "Url":
                        SetFieldUrl(item, valor, prop, addPrefix);
                        break;
                    case "DateTime":
                        SetFieldDateTime(item, valor, prop, addPrefix);
                        break;
                    case "Lookup":
                        SetFieldLookup(item, valor, prop, addPrefix);

                        break;
                    case "Taxonomy":
                        SetFieldTaxonomy(item, valor, prop, addPrefix);
                        break;
                    case "User":
                        SetFieldUser(item, web, valor, prop, addPrefix);
                        break;
                    case "Currency":
                        SetFieldCurrency(item, valor, prop, addPrefix);
                        break;
                    case "Boolean":
                        SetFieldBoolean(item, valor, prop, addPrefix);
                        break;

                    default:
                        SetFieldDefault(item, valor, prop, addPrefix);
                        break;
                }
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error SetTypeField", exception.Message));
            }
        }

        private void SetFieldDefault(SPListItem item, object valor, PropertyInfo prop, bool addPrefix)
        {
            if (!(valor.ToString().Equals("0")))
            {
                item[GetInternalName(prop.Name, addPrefix)] = valor;
            }
        }

        private void SetFieldBoolean(SPListItem item, object valor, PropertyInfo prop, bool addPrefix)
        {
            var booleano = (bool)valor;
            item[GetInternalName(prop.Name, addPrefix)] = (booleano ? "1" : "0");
        }

        private void SetFieldCurrency(SPListItem item, object valor, PropertyInfo prop, bool addPrefix)
        {
            var currency = (Currency)valor;
            if (!(currency.Value.ToString().Equals("0")))
            {
                item[GetInternalName(prop.Name, addPrefix)] = currency.Value;
            }
        }

        private void SetFieldUser(SPListItem item, SPWeb web, object valor, PropertyInfo prop, bool addPrefix)
        {
            var userList = (List<UserSP>)valor;
            var result = new StringBuilder();
            foreach (var userd in userList)
            {
                if (!string.IsNullOrEmpty(result.ToString()))
                {
                    result.Append(";");
                }
                result.Append(userd.Key);
            }
            if (userList.Count == 1)
            {
                item[GetInternalName(prop.Name, addPrefix)] = !string.IsNullOrEmpty(result.ToString())
                    ? result.ToString()
                    : string.Empty;
            }
            else
            {
                Logger.Warn("Estoy insertando un multiusuario");
                var usercollection = new SPFieldUserValueCollection();
                usercollection.AddRange(
                    userList.Select(userd => new SPFieldUserValue(web, Convert.ToInt32(userd.Key), userd.LoginName)));
                item[GetInternalName(prop.Name, addPrefix)] = usercollection;
            }
        }

        private void SetFieldTaxonomy(SPListItem item, object valor, PropertyInfo prop, bool addPrefix)
        {
            var taxonomyList = (List<Taxonomy>)valor;
            var resultTax = new StringBuilder();
            foreach (var taxonomyItem in taxonomyList)
            {
                if (!string.IsNullOrEmpty(resultTax.ToString()))
                {
                    resultTax.Append(";");
                }
                resultTax.Append("|" + taxonomyItem.Key);
            }
            item[GetInternalName(prop.Name, addPrefix)] =
                !string.IsNullOrEmpty(resultTax.ToString())
                ? resultTax.ToString()
                : string.Empty;
        }

        private void SetFieldLookup(SPListItem item, object valor, PropertyInfo prop, bool addPrefix)
        {
            var lookup = (LookupFieldEnc)valor;

            item[GetInternalName(prop.Name, addPrefix)] = new SPFieldLookupValue(Convert.ToInt32(lookup.Key), lookup.Value);
        }

        private void SetFieldDateTime(SPListItem item, object valor, PropertyInfo prop, bool addPrefix)
        {
            if (!valor.ToString().Contains("0001"))
                item[GetInternalName(prop.Name, addPrefix)] =
                    SPUtility.CreateISO8601DateTimeFromSystemDateTime((DateTime)valor);
        }

        private void SetFieldUrl(SPListItem item, object valor, PropertyInfo prop, bool addPrefix)
        {
            var urlField = (Helpers.Data.UrlField)valor;
            var urlValue = new SPFieldUrlValue
                {
                    Url = urlField.Url,
                    Description = urlField.Description
                };
            item[GetInternalName(prop.Name, addPrefix)] = urlValue;
        }

        private bool ExtractAttributes(T data, SPListItem item, PropertyInfo prop, out object valor, out string fieldType,
            out bool addPrefix)
        {
            valor = prop.GetValue(data);
            Logger.Info(string.Concat("Valor:", valor));
            fieldType = "Text";
            addPrefix = false;
            if (valor == null)
            {
                return true;
            }
            foreach (
                var enmarcha in
                    prop.GetCustomAttributes(true).Select(attribute => attribute as EnmarchaAttribute))
            {
                fieldType = enmarcha.Type.ToString();
                addPrefix = enmarcha.AddPrefeix;
            }
            var existe = true;
            try
            {
                var existField = item.Fields.GetFieldByInternalName(GetInternalName(prop.Name, addPrefix));
                if (existField == null)
                {
                    existe = false;
                }
            }
            catch (Exception exception)
            {
                existe = false;
                Logger.Error("Error" + exception.Message);
            }
            return existe;
        }

        #endregion
    }
}
