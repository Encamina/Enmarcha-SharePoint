using System;
using System.Collections.Generic;
using System.Linq;
using Enmarcha.SharePoint.Abstract.Interfaces.Artefacts;
using Enmarcha.SharePoint.Class.Logs;
using Enmarcha.SharePoint.Helpers.Data;
using Microsoft.SharePoint;

namespace Enmarcha.SharePoint.Extensors
{
    public static class RolesManagment
    {
        private static readonly ILog Logger = new LogManager().GetLogger(new System.Diagnostics.StackTrace().GetFrame(0));
        /// <summary>
        /// Create SharePoint grou
        /// </summary>
        /// <param name="web"></param>
        /// <param name="groupName">SharePoint group name</param>
        /// <param name="permissionLevel">Type of Permission</param>
        /// <returns></returns>
        public static bool CreateGroup(this SPWeb web, string groupName, SPRoleType permissionLevel)
        {
            try
            {
                var owner = web.SiteAdministrators[0];
                var member = web.SiteAdministrators[0];
                var groups = web.SiteGroups;
                groups.Add(groupName, member, owner, string.Empty);
                var newSPGroup = groups[groupName];
                var role = web.RoleDefinitions.GetByType(permissionLevel);
                var roleAssignment = new SPRoleAssignment(newSPGroup);
                roleAssignment.RoleDefinitionBindings.Add(role);
                web.RoleAssignments.Add(roleAssignment);
                web.Update();
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error Create ", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Add user of group
        /// </summary>
        /// <param name="web"></param>
        /// <param name="groupName">SharePoint group name </param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool AddUserGroup(this SPWeb web, string groupName, UserSP user)
        {
            try
            {
                var groups = web.SiteGroups;
                var groupSP = groups[groupName];
                groupSP.AddUser(web.EnsureUser(user.Value));
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error AddUserGroup ", exception.Message));
                return false;
            }
        }
        /// <summary>
        /// Remove User of Group
        /// </summary>
        /// <param name="web"></param>
        /// <param name="groupName"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool RemoveUserGroup(this SPWeb web, string groupName, UserSP user)
        {
            try
            {
                var groups = web.SiteGroups;
                var groupSP = groups[groupName];
                groupSP.RemoveUser(web.EnsureUser(user.Value));
                return true;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error RemoveUserGroup:", exception.Message));
                return false;
            }
        }

        /// <summary>
        /// Return de user exist in group
        /// </summary>
        /// <param name="web"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static IList<UserSP> GetUserInGroup(this SPWeb web, string groupName)
        {
            try
            {
                IList<UserSP> resultUser = null;
                var groups = web.SiteGroups;
                var groupSP = groups[groupName];
                var collecctionUser = groupSP.Users;
                if (collecctionUser.Count > 0)
                {
                    resultUser = new List<UserSP>();
                }
                foreach (SPUser user in collecctionUser)
                {
                    resultUser.Add(new UserSP
                    {
                        Key = user.ID.ToString(),
                        LoginName = user.LoginName,
                        Value = user.Name
                    });
                }

                return resultUser;
            }
            catch (Exception exception)
            {
                Logger.Error(string.Concat("Error GetUserInGroup:", exception.Message));
                return null;
            }
        }
        /// <summary>
        /// Get Groups by User
        /// </summary>
        /// <param name="site"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static ICollection<UserSP> GetGroupByUser(this SPSite site, string userId)
        {
            var result = new List<UserSP>();
            var spWeb = site.RootWeb;
            var groupCollection = spWeb.EnsureUser(userId).Groups;
            result.AddRange(from SPGroup @group in groupCollection
                            select new UserSP
                            {
                                Key = @group.ID.ToString(),
                                LoginName = @group.LoginName,
                                Value = @group.Name
                            });
            return result.Any() ? result : null;
        }
        ///<summary>
        /// Get the User's Groups
        ///</summary>
        ///<param name="web"></param>        
        public static IList<SPGroup> GetGroupsUser(this SPWeb web)
        {
            IList<SPGroup> groupCollection = new List<SPGroup>();

            foreach (SPGroup group in web.SiteGroups)
            {
                if (group.ContainsCurrentUser)
                {
                    groupCollection.Add(group);
                }
            }
            return groupCollection;
        }

    }
}
