using Microsoft.SharePoint.Publishing.Navigation;

namespace Enmarcha.SharePoint.Helpers.Taxonomy
{
    public sealed class TaxonomyNavigationValue
    {
        public string Name { get; set; }
        public NavigationLinkType TypeLink { get; set; }
        public string Link { get; set; }
    }
}
