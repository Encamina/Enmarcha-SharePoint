using System.Collections.Generic;
using System.Text;
using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Abstract.Interfaces.Data;

namespace Enmarcha.SharePoint.Class.Data
{
    public sealed class Query : IQuery
    {
        public string query { get; set; }
        public Query()
        {
            query = string.Empty;
        }
        public IQuery Where()
        {
            query = "<Where>{0}</Where>";
            return new Query
            {
                query = query
            };
        }

        public IQuery Concat(IQuery operacion1, IQuery operacion2, TypeOperators typeOperator)
        {
            var value=string.Empty;
            switch (typeOperator)
            {
                case TypeOperators.And:
                    value = (string.Format("<And>{0}{1}</And>", operacion1.Execute(), operacion2.Execute()));
                    break;
                case TypeOperators.Or:
                default:
                    value = (string.Format("<Or>{0}{1}</Or>", operacion1.Execute(), operacion2.Execute()));
                    break;              
            }
            query = string.IsNullOrEmpty(query) ? value : query.Replace("{0}", value);

            return new Query
            {
                query = query
            };
        }

        public IQuery Field(string field,string extra)
        {
            query = string.IsNullOrEmpty(query) 
                ? 
                string.Format(@"<Op><FieldRef Name='{0}' {1} />Val</Op>",
                    string.Concat("", field), extra) :
                        query.Replace("{0}", 
                        string.Format(@"<Op><FieldRef Name='{0}' {1} />Val</Op>",
                        string.Concat("", field),
                        extra));
            return new Query
            {
                query = query
            };
        }

        public IQuery Value(string typeField,string value)
        {
            query = query.Replace("Val", string.Format(@"<Value Type='{1}'>{0}</Value>", value, typeField));
            return new Query
            {
                query = query
            };
        }

        public IQuery Operator(TypeOperators typTypeOperators)
        {
            query = query.Replace("Op", typTypeOperators.ToString());
            return new Query
            {
                query = query
            };
        }

        public IQuery OrderBy(IDictionary<string, string> fields)
        {
            var sb = new StringBuilder();
            sb.Append("<OrderBy>");
            foreach (KeyValuePair<string, string> field in fields)
            {
                sb.AppendFormat("<FieldRef Name='{0}' Ascending='{1}'></FieldRef>", string.Concat("", field.Key), field.Value);
            }
            sb.Append("</OrderBy>");
            query += (sb.ToString());
            return new Query
            {
                query = query
            };
        }

        public IQuery GroupBy(IDictionary<string, string> fields)
        {
            var sb = new StringBuilder();
            sb.Append("<GroupBy Collapse='TRUE'>");
            foreach (var field in fields)
            {
                sb.AppendFormat("<FieldRef Name='{0}'/>", string.Concat("", field.Key));
            }
            sb.Append("</GroupBy>");
            query += (sb.ToString());
            return new Query
            {
                query = query
            };
        }

        public IQuery ViewFields(IDictionary<string, string> fields)
        {
            var sb = new StringBuilder();
            sb.Append("<ViewFields>");
            foreach (var field in fields)
            {
                sb.AppendFormat("<FieldRef Name='{0}'/>", string.Concat("", field.Key));
            }
            sb.Append("</ViewFields>");
            query += (sb.ToString());
            return new Query
            {
                query = query
            };
        }

        public string Execute()
        {
            return query;
        }
    }
}
