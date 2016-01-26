using System.Collections.Generic;
using Enmarcha.SharePoint.Abstract.Entities;

namespace Enmarcha.SharePoint.Abstract.Class
{
    public sealed class  TaxonomyValue
    {
        public string Name { get; set; }
        public IList<CustomProperties> Properties { get; set; }
    }
}
