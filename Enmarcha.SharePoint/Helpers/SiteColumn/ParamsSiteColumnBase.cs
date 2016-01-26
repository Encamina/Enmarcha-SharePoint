
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Microsoft.SharePoint;

namespace Enmarcha.SharePoint.Helpers.SiteColumn
{
    public class ParamsSiteColumnBase
    {

        public SPWeb Web { get; set; }
        public bool AddPrefix { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public ILog Logger { get; set; }
    }
}
