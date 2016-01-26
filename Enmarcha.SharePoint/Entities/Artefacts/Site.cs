using System;
using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Microsoft.SharePoint;

namespace Enmarcha.SharePoint.Entities.Artefacts
{
    public  sealed class Site : ISite
    {
        #region Properties
        public SPWeb Web { get; set; }
        public ILog Logger { get; set; }
        #endregion
        #region Constructors
        public Site(SPWeb web,ILog logger)
        {
            Web = web;
            Logger = logger;
        }
        #endregion
        /// <summary>
        /// Add Permission in a Site
        /// </summary>
        /// <param name="group"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool AddPermision(string group, RoleType role)
        {
            try
            {                
                var oGroup = Web.Site.RootWeb.Groups.GetByName(group);
                var roleAssignment = new SPRoleAssignment(oGroup);
                var roleDefinition = Web.RoleDefinitions.GetByType((SPRoleType) role);
                roleAssignment.RoleDefinitionBindings.Add(roleDefinition);
                if (!Web.HasUniqueRoleAssignments)
                {
                    Web.BreakRoleInheritance(false);
                }
                Web.RoleAssignments.Add(roleAssignment);
                Web.Update();                
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddPermission :", exception.Message));                
                return false;
            }
        }

        /// <summary>
        /// Create Site in SharePoint 
        /// </summary>
        /// <param name="web"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="template"></param>
        /// <param name="lcid"></param>
        /// <returns></returns>
        public bool CreateSite(string web, string title, string description, string template, uint lcid)
        {
            return CreateSite(web, title, description, template, lcid, false);
        }

        /// <summary>
        /// Create Site in SharePoint 
        /// </summary>
        /// <param name="web"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="template"></param>
        /// <param name="lcid"></param>
        /// <param name="breakPermisions"></param>
        /// <returns></returns>
        public bool CreateSite(string web, string title, string description, string template, uint lcid, bool breakPermisions)
        {
            try
            {
                using (var result = Web.Webs.Add(web, title, description, lcid, template, false, false))
                {
                    if (!breakPermisions)
                    {
                        return true;
                    }
                    result.BreakRoleInheritance(false);
                    result.Update();
                }
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Create Site: ", exception.Message));
                return false;
            }
        }
        /// <summary>
        /// Remove Permissions a group in SharePoint
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool RemovePermision(string group)
        {
            try
            {                
                var oGroup = Web.Site.RootWeb.Groups.GetByName(group);
                Web.RoleAssignments.RemoveById(oGroup.ID);
                Web.Update();             
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddPermission :", exception.Message));                
                return false;
            }
        }
    }
}
