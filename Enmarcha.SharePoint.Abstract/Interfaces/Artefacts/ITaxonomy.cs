using System.Collections.Generic;
using Enmarcha.SharePoint.Abstract.Class;

namespace Enmarcha.SharePoint.Abstract.Interfaces.Artefacts
{
    public interface ITaxonomy
    {
        IDictionary<string, string> GetAllTerms();
        bool AddGroup(string group);
        bool AddTerms(string name);
        bool AddTerms(string name, bool naviagation);
        IDictionary<string, TaxonomyValue> GetTerms(string term);
        IDictionary<string, TaxonomyValue> GetSubTerms(string term, string subtTerm);


    }
}
