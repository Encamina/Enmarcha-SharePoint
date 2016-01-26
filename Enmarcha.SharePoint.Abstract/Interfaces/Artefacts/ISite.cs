using Enmarcha.SharePoint.Abstract.Enum;

namespace Enmarcha.SharePoint.Abstract.Interfaces.Artefacts
{
    public interface ISite
    {
        bool CreateSite(string web, string title, string description, string template, uint lcid);
        bool CreateSite(string web, string title, string description, string template, uint lcid, bool breakPermisions);
        bool AddPermision(string group, RoleType role);
        bool RemovePermision(string group);
    }
}
