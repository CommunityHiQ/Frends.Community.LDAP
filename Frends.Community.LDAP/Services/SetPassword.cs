using System;
using System.DirectoryServices.AccountManagement;

namespace Frends.Community.LDAP.Services
{
    class SetPassword
    {
        internal static void SetUserPassword(string ldapUri, string OUPath, string username, string loginPassword, string CNValue, string newPassword) 
        {
            try
            {
                ldapUri = ldapUri.ToLower().Replace("ldap://", "");
                CNValue = CNValue.ToLower().Replace("cn=", "");
                using (var oPrincipalContext = new PrincipalContext(ContextType.Domain, ldapUri, OUPath,
                            ContextOptions.SimpleBind, username, loginPassword))
                {
                    var user = UserPrincipal.FindByIdentity(oPrincipalContext, CNValue);

                    if (user == null)
                    {
                        throw new Exception("Could not find user: " + CNValue +". ");

                    }
                    user.SetPassword(newPassword);
                }
            }

            catch (Exception ex)
            {
                throw new Exception("Password could not be set.", ex);
            }
        }
          
    }
}
