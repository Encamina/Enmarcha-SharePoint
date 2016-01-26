using System.Collections.Generic;
using Enmarcha.SharePoint.Abstract.Enum;

namespace Enmarcha.SharePoint.Abstract.Interfaces.Data
{
    public interface IQuery
    {
        IQuery Where();
        IQuery Concat(IQuery operacion1, IQuery operacion2, TypeOperators typeTypeOperator);
        IQuery Field(string field, string extra);
        IQuery Value(string typeField, string value);
        IQuery Operator(TypeOperators typTypeOperators);
        IQuery OrderBy(IDictionary<string, string> fields);
        IQuery GroupBy(IDictionary<string, string> fields);
        IQuery ViewFields(IDictionary<string, string> fields);
        string Execute();
    }
}
