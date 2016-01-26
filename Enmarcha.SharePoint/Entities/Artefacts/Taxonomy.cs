using System;
using System.Collections.Generic;
using System.Linq;
using Enmarcha.SharePoint.Abstract.Class;
using Enmarcha.SharePoint.Abstract.Entities;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Helpers.Taxonomy;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing.Navigation;
using Microsoft.SharePoint.Taxonomy;

namespace Enmarcha.SharePoint.Entities.Artefacts
{
   public sealed class Taxonomy:ITaxonomy
    {
        #region properties

        public SPSite SpSite { get; set; }
        public string NameTermStore { get; set; }
        public string NameGroup { get; set; }
        public bool Navigation { get; set; }
        public ILog Logger { get; set; }
        #endregion
        #region Constructor

        public Taxonomy(SPSite site, ILog logger, string termStore) : this(site, logger, termStore, string.Empty, false)
        {
        }

        public Taxonomy(SPSite site,ILog logger, string termStore, string group) : this(site, logger, termStore, group, false)
       {
       }

       public Taxonomy(SPSite site, ILog logger, string termStore, string group, bool navigation )
        {
            SpSite = site;
            NameTermStore = termStore;
            NameGroup = group;
            Navigation = navigation;
           Logger = logger;
        }
        #endregion
        #region Interface
        /// <summary>
        ///GetAll Termes Taxonomy
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> GetAllTerms()
        {
            try
            {
                IDictionary<string, string> result = new Dictionary<string, string>();
                var session = new TaxonomySession(SpSite);
                var termStore = session.TermStores[NameTermStore];
                var group = termStore.Groups[NameGroup];
                foreach (var termSet in group.TermSets)
                {
                    result.Add(termSet.Id.ToString(), termSet.Name);
                }
                return result;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error GetAllTerms", exception.Message));
                return null;
            }
        }

        /// <summary>
        /// Function that returns a term given us all the values ​​within the taxonomy
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public IDictionary<string, TaxonomyValue> GetTerms(string term)
        {
            try
            {

                var session = new TaxonomySession(SpSite);
                var termStore = session.TermStores[NameTermStore];
                var group = termStore.Groups[NameGroup];
                var terms = group.TermSets[term];
                if (terms.Terms.Count == 0)
                {
                    return null;
                }
                var result = new Dictionary<string, TaxonomyValue>();
                foreach (var item in terms.Terms)
                {
                    var listCustomProperties = item.CustomProperties.Select(itemProperties => new CustomProperties
                    {
                        Name = itemProperties.Key,
                        Value = itemProperties.Value
                    }).ToList();

                    result.Add(item.Id.ToString(),
                        new TaxonomyValue { Name = item.Name, Properties = listCustomProperties });
                }
                return result;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error GetAllTerms", exception.Message));
                return null;
            }
        }

        /// <summary>
        /// Add Terms
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
       public bool AddTerms(string name)
       {
           return AddTerms(name,false);
       }
       /// <summary>
       /// Add Terms
       /// </summary>
       /// <param name="name"></param>
       /// <param name="navigation"></param>
       /// <returns></returns>
       public bool AddTerms(string name,bool navigation)
        {
            try
            {
                var session = new TaxonomySession(SpSite);
                var termStore = session.TermStores[NameTermStore];
                var group = termStore.Groups[NameGroup];

                var term = group.CreateTermSet(name);
                term.IsOpenForTermCreation = true;
                if (navigation)
                {
                    var navTermSet = NavigationTermSet.GetAsResolvedByWeb(term, SpSite.RootWeb, StandardNavigationProviderNames.GlobalNavigationTaxonomyProvider);
                    navTermSet.IsNavigationTermSet = true;
                }
                termStore.CommitAll();

                return true;
            }
            catch (Exception exception)
            {
               Logger.Error(string.Concat("Error AddTerms", exception.Message));
                return false;
            }
        }


        /// <summary>
        /// Function return the subterms of the term
        /// </summary>
        /// <param name="term"></param>
        /// <param name="subtTerm"></param>
        /// <returns></returns>
        public IDictionary<string, TaxonomyValue> GetSubTerms(string term, string subtTerm)
        {
            try
            {
                var session = new TaxonomySession(SpSite);
                var termStore = session.TermStores[NameTermStore];
                var group = termStore.Groups[NameGroup];
                var terms = group.TermSets[term];
                var subTerms = terms.Terms[subtTerm];
                if (subTerms.Terms.Count == 0)
                {
                    return null;
                }
                var result = new Dictionary<string, TaxonomyValue>();
                foreach (var item in subTerms.Terms)
                {
                    var listCustomProperties = item.CustomProperties.Select(itemProperties => new CustomProperties
                    {
                        Name = itemProperties.Key,
                        Value = itemProperties.Value
                    }).ToList();

                    result.Add(item.Id.ToString(),
                        new TaxonomyValue { Name = item.Name, Properties = listCustomProperties });
                }
                return result;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error GetAllTerms", exception.Message));
                return null;
            }
        }
        /// <summary>
        /// Add Group to Taxonomy
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public bool AddGroup(string groupName)
        {
            try
            {
                var session = new TaxonomySession(SpSite);
                var termStore = session.TermStores[NameTermStore];
                if (ExistGroup(groupName))
                {
                    return true;
                }
                termStore.CreateGroup(groupName);
                termStore.CommitAll();
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddGroup", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Add New Terms 
        /// </summary>
        /// <param name="term"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Term AddNewTerms(string term, string name)
        {
            try
            {
                var session = new TaxonomySession(SpSite);
                var termStore = session.TermStores[NameTermStore];
                var group = termStore.Groups[NameGroup];
                TermSet termSet;
                try
                {
                    termSet = group.TermSets[term];
                }
                catch (Exception)
                {
                    termSet = group.CreateTermSet(term);
                    termSet.IsOpenForTermCreation = true;
                    if (Navigation)
                    {
                        var navTermSet = NavigationTermSet.GetAsResolvedByWeb(termSet, SpSite.RootWeb, StandardNavigationProviderNames.GlobalNavigationTaxonomyProvider);
                        navTermSet.IsNavigationTermSet = true;
                    }
                }

                var result = termSet.CreateTerm(name, (int)SpSite.RootWeb.Language);
                termStore.CommitAll();
                return result;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddNewTerms", exception.Message));
                return null;
            }
        }

        /// <summary>
        /// Add Terms and Subterms
        /// </summary>
        /// <param name="term"></param>
        /// <param name="subTerms"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddTerms(string term, string subTerms, string name)
        {
            try
            {
                TermStore termStore;
                var termSet = GetTermSet(term, out termStore);
                Term termTaxonomy;
                try
                {
                    termTaxonomy = termSet.Terms[subTerms];
                }
                catch (Exception)
                {
                    termTaxonomy = termSet.CreateTerm(subTerms, SpSite.RootWeb.CurrencyLocaleID);
                }
                termTaxonomy.CreateTerm(name, SpSite.RootWeb.CurrencyLocaleID);
                termStore.CommitAll();
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error GetAllTerms", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Add CustomProperties in a Term to Taxonomy
        /// </summary>
        /// <param name="term"></param>
        /// <param name="name"></param>
        /// <param name="propertyName"></param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        public bool AddCustomPropertiesTermTaxonomy(string term, string name, string propertyName, string propertyValue)
        {
            try
            {
                var session = new TaxonomySession(SpSite);

                var termStore = session.TermStores[NameTermStore];
                var group = termStore.Groups[NameGroup];
                var termSet = @group.TermSets[term];
                var termUpdate = termSet.Terms[name];
                termUpdate.SetCustomProperty(propertyName, propertyValue);
                termStore.CommitAll();
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddCustomPropertiesTermTaxonomy", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Function to add terms Navigation
        /// </summary>
        /// <param name="term"></param>        
        /// <param name="navigation">Objeto Navegación</param>
        /// <returns></returns>
        public bool AddTermsNavigation(string term, TaxonomyNavigationValue navigation)
        {
            try
            {
                var session = new TaxonomySession(SpSite);
                var termStore = session.TermStores[NameTermStore];
                var group = termStore.Groups[NameGroup];
                TermSet termSet;
                try
                {
                    termSet = group.TermSets[term];
                }
                catch (Exception)
                {

                    termSet = group.CreateTermSet(term);

                }
                var navTermSet = NavigationTermSet.GetAsResolvedByWeb(termSet, SpSite.RootWeb, StandardNavigationProviderNames.GlobalNavigationTaxonomyProvider);
                navTermSet.IsNavigationTermSet = true;
                if (navigation != null)
                {
                    var termNav = navTermSet.CreateTerm(navigation.Name, navigation.TypeLink);
                    termNav.SimpleLinkUrl = navigation.Link;
                }
                termStore.CommitAll();
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error GetAllTerms", exception.Message));
                return false;
            }
        }

        #endregion

        #region Methods Private

        /// <summary>
        /// Return te TermSet
        /// </summary>
        /// <param name="term"></param>
        /// <param name="termStore"></param>
        /// <returns></returns>
        private TermSet GetTermSet(string term, out TermStore termStore)
        {
            var session = new TaxonomySession(SpSite);
            termStore = session.TermStores[NameTermStore];
            var group = termStore.Groups[NameGroup];
            TermSet termSet;
            try
            {
                termSet = @group.TermSets[term];
            }
            catch (Exception)
            {
                termSet = @group.CreateTermSet(term);
            }
            return termSet;
        }

        /// <summary>
        /// Checks for the group
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        private bool ExistGroup(string groupName)
        {
            try
            {
                var session = new TaxonomySession(SpSite);
                var termStore = session.TermStores[NameTermStore];
                var exist = termStore.Groups[groupName];
                return exist != null;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddGroup", exception.Message));
                return false;
            }
        }

        #endregion
    }
}
