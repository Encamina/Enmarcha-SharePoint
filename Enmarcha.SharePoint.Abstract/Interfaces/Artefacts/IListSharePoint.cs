using System.Collections.Generic;
using Enmarcha.SharePoint.Abstract.Enum;

namespace Enmarcha.SharePoint.Abstract.Interfaces.Artefacts
{
    public interface IListSharePoint
    {
        bool Create(string description, ListTemplateType type, bool versionControl);
        bool Delete();
        bool Exist();
        bool AddContentType(string contentTypeName);
        bool DeleteContentType(string contentTypeName);
         
        bool AddPermissionsGroup(string group, RoleType role);
        bool RemovePermissionsGroup(string group);
        bool ClearPermisions();
        bool CreateFolder(string name);
        IEnumerable<string> GetContentType();
    }
}
