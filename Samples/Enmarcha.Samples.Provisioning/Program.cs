using Enmarcha.Samples.Provisioning.Model;
using Enmarcha.SharePoint.Abstract.Enum;
using Enmarcha.SharePoint.Entities.Artefacts;
using Enmarcha.SharePoint.Extensors;
using Microsoft.SharePoint;

namespace Enmarcha.Samples.Provisioning
{
    class Program
    {
        static void Main(string[] args)
        {
            const string urlSharePointOnpremise = "[enteryoururlhere]";          
            using (var site = new SPSite(urlSharePointOnpremise))
            {
                using (var web = site.OpenWeb())
                {
                   

                    var columnSite = web.CreateColumnSite("ENMARCHA ContentType", typeof(Employed));
                   web.CreateContentType("EmployedBussines", "ENMARCHA","Elemento", columnSite);

                    web.CreateList("Empleados", "Lista de Empleados", TypeList.GenericList, true);
                    var list = web.Lists.TryGetList("Empleados");                    
                     list.AddContentTypeLibrary("EmployedBussines");
                }
            }
        }
    }
}
