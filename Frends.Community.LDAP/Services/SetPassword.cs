using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.Community.LDAP.Services
{
    class SetPassword
    {
        static internal void SetUserPassword(string ldapUri, string OUPath, string username, string loginPassword, string CNValue, string newPassword) 
        {
            try
            {
                ldapUri = ldapUri.ToLower().Replace("ldap://", "");
                CNValue = CNValue.ToLower().Replace("cn=", "");
                using (var oPrincipalContext = new PrincipalContext(ContextType.Domain, ldapUri, OUPath,
                            ContextOptions.SimpleBind, username, loginPassword))
                {
                    var user = UserPrincipal.FindByIdentity(oPrincipalContext, CNValue);
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
