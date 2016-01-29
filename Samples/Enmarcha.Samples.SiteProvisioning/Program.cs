using System;
using Microsoft.SharePoint;
using Enmarcha.SharePoint.Extensors;

namespace Enmarcha.Samples.SiteProvisioning
{
    class Program
    {
        static void Main(string[] args)
        {
            const string urlSharePointOnpremise = "urlsiteSharePoint";

            using (var site = new SPSite(urlSharePointOnpremise))
            {
                Console.WriteLine("Creating sample site");
                var siteCreationSuccess = site.CreateSite("sample", "Sample Site", "This is a sample site", "STS");
                Console.WriteLine("Sample site creaction success");

                Console.WriteLine("Creating sample subsite");
                using (var web = site.OpenWeb("sample"))
                {
                    var subSiteCreationSuccess = web.CreateSubSite("samplesubsite", "Sample Subsite", "This is a sample subsite", "STS");
                    Console.WriteLine("Sample subsite creation success");
                }

            }

            Console.ReadLine();
        }
    }
}
