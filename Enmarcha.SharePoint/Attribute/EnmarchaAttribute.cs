using Enmarcha.SharePoint.Abstract.Enum;

namespace Enmarcha.SharePoint.Attribute
{    
    public sealed class EnmarchaAttribute : System.Attribute
    {
        public EnmarchaAttribute()
        {
            Create = true;
            SourceList = string.Empty;
            Choice = null;
            Term = string.Empty;
            MultiValue = false;
            ValueDefault = string.Empty;
            AddPrefeix = true;
            Hidden = false;
            DisplayName = string.Empty;
            Required = false;
            Group = "ENCAMINA";
            Formula = string.Empty;
            ReadOnly = false;
            Date = TypeDate.DateTime;
            FormatCurrency = 0;

        }
        public TypeField Type { get; set; }
        public string Formula { get; set; }
        public bool ReadOnly { get; set; }
        public bool Create { get; set; }
        public string Group { get; set; }
        public string[] Choice { get; set; }
        public string SourceList { get; set; }
        public string Term { get; set; }
        public bool MultiValue { get; set; }
        public string ValueDefault { get; set; }
        public bool AddPrefeix { get; set; }
        public string DisplayName { get; set; }
        public bool Hidden { get; set; }
        public bool Required { get; set; }
        public TypeDate Date { get; set; }
        public int FormatCurrency { get; set; }
    }
}
