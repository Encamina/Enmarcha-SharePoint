using System.Collections.Specialized;
using Enmarcha.SharePoint.Abstract.Enum;
using Microsoft.SharePoint;

namespace Enmarcha.SharePoint.Helpers.Extensors
{
    public sealed class ParamsCreateSite
    {
        public SPWeb Web { get; set; }
        public string Name { get; set; }
        public string FieldType { get; set; }
        public bool Requiered { get; set; }
        public string DefaultValue { get; set; }
        public StringCollection Choice { get; set; }
        public string Group { get; set; }
        public string Term { get; set; }
        public bool MultiValue { get; set; }
        public string DisplayName { get; set; }
        public bool Hidden { get; set; }
        public string GroupTerm { get; set; }
        public bool AddPrefix { get; set; }
        public string Formula { get; set; }
        public int FormatCurrency { get; set; }
        public TypeDate Date { get; set; }
    }

}
