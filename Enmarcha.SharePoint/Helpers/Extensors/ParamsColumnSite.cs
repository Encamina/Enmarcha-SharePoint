using System.Collections.Specialized;
using Enmarcha.SharePoint.Entities.Artefacts;

namespace Enmarcha.SharePoint.Helpers.Extensors
{
    public class ParamsColumnSite
    {
        public ListSharePoint List { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string FieldType { get; set; }
        public bool Requiered { get; set; }
        public string DefaultValue { get; set; }
        public StringCollection Choice { get; set; }
        public string Group { get; set; }
        public bool MultiValue { get; set; }
        public bool AddPrefix { get; set; }
    }
}
