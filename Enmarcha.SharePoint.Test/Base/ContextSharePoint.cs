using System;
using System.Configuration;
using System.Security;
using Microsoft.SharePoint;

namespace Enmarcha.SharePoint.Test.Base
{
    internal static class ContextSharePoint    
        {
            #region Constructor
            static ContextSharePoint()
            {
                UserName = ConfigurationManager.AppSettings["OnPremUserName"];
                var password = ConfigurationManager.AppSettings["OnPremPassword"];
                Password = GetSecureString(password);
                Domain = ConfigurationManager.AppSettings["OnPremDomain"];
                TenantUrl = ConfigurationManager.AppSettings["OnPremSiteCollection"];

            }
            #endregion

            #region Properties
            private static string TenantUrl { get; set; }
            private static string UserName { get; set; }
            private static SecureString Password { get; set; }
            private static string Domain { get; set; }



            #endregion

            #region Methods
            public static SPSite CreateClientContext()
            {
                try
                {
                    var site = new SPSite(TenantUrl);
                    return site;

                }
                catch (Exception)
                {

                    return null;
                }
            }

            private static SecureString GetSecureString(string input)
            {
                if (string.IsNullOrEmpty(input))
                    throw new ArgumentException("Input string is empty and cannot be made into a SecureString", "input");

                var secureString = new SecureString();
                foreach (var c in input)
                {
                    secureString.AppendChar(c);
                }
                return secureString;
            }

        public static bool VerifyServer(SPSite site)
        {
            return site != null;
        }

        #endregion
        
    }
}
