using System;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Microsoft.SharePoint;

namespace Enmarcha.SharePoint.Entities.Artefacts
{
    public sealed class MasterPage
    {
        #region Properties

        public SPWeb Web { get; set; }
        public ILog Logger { get; set; }

        #endregion

        #region Constructores

        public MasterPage(SPWeb web,ILog logger)
        {
            Web = web;
            Logger = logger;
        }

        #endregion

        #region Interface
       /// <summary>
       /// Return the Url of the Master Page
       /// </summary>
       /// <returns></returns>
        public string GetMasterPage()
        {
            return Web.Site.RootWeb.MasterUrl;
        }

        /// <summary>
        /// Set de MasterPage of Default
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool SetMasterPage(string name)
        {
            try
            {
                Web.Site.RootWeb.CustomMasterUrl = name;
                Web.Site.RootWeb.MasterUrl = name;
                Web.Update();
                return true;
            }
            catch (Exception excepcion)
            {
                Logger.Error(string.Concat("Error Set MasterPage:", excepcion.Message));
                return false;
            }
        }

        #endregion
    }
}
