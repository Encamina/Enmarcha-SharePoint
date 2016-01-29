using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Extensors;
using Microsoft.SharePoint;

namespace Enmarcha.Samples.PermissionList
{
    class Program
    {
        static void Main(string[] args)
        {
            const string urlSharePointOnpremise = "urlsiteSharePoint";
            const string listSample = "listSample";
            using (var site = new SPSite(urlSharePointOnpremise))
            {
                using (var web = site.OpenWeb())
                {
                    var list = web.Lists.TryGetList(listSample);
                    if (list == null) web.CreateList(listSample, string.Empty, TypeList.GenericList,true);
                    web.CreateGroup("Test",SPRoleType.Reader);
                var result=    list.AddPermisionLibrary("Test", RoleType.Contributor);


                }
            }
        }
    }
}
