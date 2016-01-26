using System;
using System.Collections.Generic;
using Enmarcha.SharePoint.Abstract.Class;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Class.Logs;
using Enmarcha.SharePoint.Helpers.Extensors;
using Enmarcha.SharePoint.Helpers.Taxonomy;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;

namespace Enmarcha.SharePoint.Extensors
{
    public static class Taxonomy
    {
        private static readonly ILog Logger = new LogManager().GetLogger(new System.Diagnostics.StackTrace().GetFrame(0));
        /// <summary>
        /// Get Terms of Taxonomy
        /// </summary>
        /// <param name="site"></param>
        /// <param name="termStore"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static IDictionary<string, string> GetTaxonomy(this SPSite site, string termStore, string group)
        {
            IDictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                var taxonomy = new Entities.Artefacts.Taxonomy(site, Logger, termStore, group);
                result = taxonomy.GetAllTerms();
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Get Taxonomy", exception.Message));
                return null;
            }
            return (result.Count > 0) ? result : null;
        }

        /// <summary>
        /// Get Terms
        /// </summary>
        /// <param name="site"></param>
        /// <param name="termStore"></param>
        /// <param name="group"></param>
        /// <param name="terms"></param>
        /// <returns></returns>
        public static IDictionary<string, TaxonomyValue> GetTems(this SPSite site, string termStore, string group, string terms)
        {
            try
            {
                var taxonomy = new Entities.Artefacts.Taxonomy(site, Logger, termStore, group);
                return taxonomy.GetTerms(terms);

            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Get Taxonomy", exception.Message));
                return null;
            }
        }
        /// <summary>
        /// Get SubTerms
        /// </summary>
        /// <param name="site"></param>
        /// <param name="termStore"></param>
        /// <param name="group"></param>
        /// <param name="terms"></param>
        /// <param name="subTerm"></param>
        /// <returns></returns>
        public static IDictionary<string, TaxonomyValue> GetSubTems(this SPSite site, string termStore, string group, string terms, string subTerm)
        {
            try
            {
                var taxonomy = new Entities.Artefacts.Taxonomy(site, Logger, termStore, group);
                return taxonomy.GetSubTerms(terms, subTerm);

            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Get Taxonomy", exception.Message));
                return null;
            }
        }
        /// <summary>
        /// Add Term in a group
        /// </summary>
        /// <param name="site"></param>
        /// <param name="termStore"></param>
        /// <param name="group"></param>
        /// <returns></returns>
        public static bool AddTermGroup(this SPSite site, string termStore, string @group)
        {
            try
            {
                var taxonomy = new Entities.Artefacts.Taxonomy(site, Logger, termStore, group);
                return taxonomy.AddGroup(group);
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddGroup", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Add Items in Taxonomy
        /// </summary>
        /// <param name="site"></param>
        /// <param name="termStore"></param>
        /// <param name="group"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool AddTaxonomy(this SPSite site, string termStore, string group, string name)
        {
            try
            {
                var taxonomy = new Entities.Artefacts.Taxonomy(site, Logger, termStore, group);
                return taxonomy.AddTerms(name);
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddTaxonomy", exception.Message));
                return false;
            }
        }


        /// <summary>
        /// Add Term in a TermStore
        /// </summary>
        /// <param name="site"></param>
        /// <param name="termStore"></param>
        /// <param name="group"></param>
        /// <param name="term"></param>
        /// <param name="name"></param>
        /// <param name="navigation"></param>
        /// <returns></returns>
        public static Term AddTermTaxonomy(this SPSite site, string termStore, string @group, string term, string name)
        {
            return AddTermTaxonomy(site, termStore, @group, term, name, false);
        }

        /// <summary>
        /// Add Term in a TermStore
        /// </summary>
        /// <param name="site"></param>
        /// <param name="termStore"></param>
        /// <param name="group"></param>
        /// <param name="term"></param>
        /// <param name="name"></param>
        /// <param name="navigation"></param>
        /// <returns></returns>
        public static Term AddTermTaxonomy(this SPSite site, string termStore, string @group, string term, string name, bool navigation)
        {
            try
            {
                var taxonomy = new Entities.Artefacts.Taxonomy(site, Logger, termStore, group, navigation);
                return taxonomy.AddNewTerms(term, name);
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddTaxonomy", exception.Message));
                return null;
            }

        }
        /// <summary>
        /// Add SubTerms in Store
        /// </summary>
        /// <param name="site"></param>
        /// <param name="termStore"></param>
        /// <param name="group"></param>
        /// <param name="term"></param>
        /// <param name="subTerm"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool AddSubTermTaxonomy(this SPSite site, string termStore, string group, string term, string subTerm, string name)
        {
            try
            {
                var taxonomy = new Entities.Artefacts.Taxonomy(site, Logger, termStore, group);
                return taxonomy.AddTerms(term, subTerm, name);

            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddTaxonomy", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Add CustomProperties to de Taxonomy
        /// </summary>
        /// <param name="site"></param>
        /// <param name="termStore"></param>
        /// <param name="group"></param>
        /// <param name="term"></param>
        /// <param name="name"></param>
        /// <param name="listCustomProperties"></param>
        /// <returns></returns>
        public static bool AddCustomPropertiesTermTaxonomy(this SPSite site, string termStore, string @group, string term, string name,
            IList<CustomProperty> listCustomProperties)
        {
            try
            {
                var taxonomy = new Entities.Artefacts.Taxonomy(site, Logger, termStore, group);
                foreach (var customProperties in listCustomProperties)
                {
                    taxonomy.AddCustomPropertiesTermTaxonomy(term, name, customProperties.Name,
                        customProperties.Value);
                }
                return true;

            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddCustomPropertiesTermTaxonomy", exception.Message));
                return false;
            }
        }
        /// <summary>
        /// Add the terms of navigation
        /// </summary>
        /// <param name="site"> Sitio</param>
        /// <param name="termStore"> Nombre del servicio de metadatos </param>
        /// <param name="group">Group </param>
        /// <param name="term"> Termino </param>
        /// <param name="navigationList">Lista con los elementos de Navegacion</param>
        /// <returns></returns>
        public static bool AddTermNavigation(this SPSite site, string termStore, string @group, string term, TaxonomyNavigationValue navigationList)
        {
            try
            {
                var taxonomy = new Entities.Artefacts.Taxonomy(site,Logger, termStore, group);
                return taxonomy.AddTermsNavigation(term, navigationList);

            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddTermNavigation", exception.Message));
                return false;
            }

        }
    }
}
