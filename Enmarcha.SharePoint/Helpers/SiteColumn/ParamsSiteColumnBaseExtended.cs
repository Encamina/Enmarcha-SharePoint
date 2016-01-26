using Microsoft.SharePoint;

namespace Enmarcha.SharePoint.Helpers.SiteColumn
{
    public  class ParamsSiteColumnBaseExtended:ParamsSiteColumnBase
    {
        public SPFieldType FieldType { get; set; }
        public bool Requiered { get; set; }
        public string Group { get; set; }
        public bool MultiValue { get; set; }
    }
}
