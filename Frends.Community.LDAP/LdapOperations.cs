using Frends.Community.LDAP.Models;
using Frends.Community.LDAP.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.Community.LDAP
{
    public static class LdapOperations
    {
        /// <summary>
        /// Options class for AD operations.
        /// 
        /// Fields and their descriptions:
        /// 
        /// - attribute: AD User exists: The name of the attribute to search by.
        /// - value: AD User exists: The value of the attribute to search by.
        /// - setPassword: AD Create user: Defines if password should be set at create time.
        /// - newPassword: AD Create user: Defines the new password if needed.
        /// - groups: Add to groups tasks: To which groups the user should be added.
        /// </summary>
        public class Options
        {
            public string attribute;
            public string value;
            public bool setPassword;

            [PasswordPropertyText(true)]
            public string newPassword;

            public string[] groups;
        }

        /// <summary>
        /// Result class for all tasks.
        /// 
        /// Fields and their descriptions:
        /// 
        /// - operationSuccessful: Tells if the requested operation was performed successfully.
        /// - user: If the operation returns a user entry, it is passed in this variable.
        /// </summary>
        public class Output
        {
            public bool operationSuccessful;
            public DirectoryEntry user;
        }

        /// <summary>
        /// Searches Active Directory for user(s) specified by the given attribute and its value, included in the InputOtherData class.
        /// </summary>
        /// <param name="ldapConnectionInfo">The LDAP connection information</param>
        /// <param name="otherData">Other data needed for the query</param>
        /// <returns>LdapResult class</returns>
        public static Output AD_UserExists(LdapConnectionInfo ldapConnectionInfo, Options otherData)
        {
            var ldapOperationResult = new Output { operationSuccessful = false, user = null };

            DirectoryEntry user;
            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                user = ldap.SearchUser(otherData.attribute, otherData.value);
            }

            ldapOperationResult.operationSuccessful = !(user == null);

            return ldapOperationResult;
        }

        /// <summary>
        /// Create a user to AD.
        /// </summary>
        /// <param name="ldapConnectionInfo">The LDAP connection information</param>
        /// <param name="adUser">The user record to be created</param>
        /// <param name="otherData">Passes two parameters to this task: bool setPassword, which defines if a password should be set at create time, and string newPassword, containing the password to be set.</param>
        /// <returns>LdapResult class, which carries a copy of the created user record.</returns>
        public static Output AD_CreateUser(LdapConnectionInfo ldapConnectionInfo, AdUser adUser, Options otherData )
        {
            var ldapOperationResult = new Output { operationSuccessful = false, user = null };

            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                ldapOperationResult.user = ldap.CreateAdUser(adUser);

                if (otherData.setPassword) 
                {
                    SetPassword.SetUserPassword(ldapConnectionInfo.LdapUri,adUser.OU,ldapConnectionInfo.Username,ldapConnectionInfo.Password,adUser.CN, otherData.newPassword);
                }

                ldapOperationResult.operationSuccessful = true;

                return ldapOperationResult;
            }   
        }

        /// <summary>
        /// Update a user in the AD.
        /// </summary>
        /// <param name="ldapConnectionInfo">The LDAP connection information</param>
        /// <param name="adUser">The user record to be updated</param>
        /// <returns>LdapResult class, which carries a copy of the updated user record.</returns>
        public static Output AD_UpdateUser(LdapConnectionInfo ldapConnectionInfo, AdUser adUser)
        {
            var ldapOperationResult = new Output { operationSuccessful = false, user = null };

            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                ldapOperationResult.user = ldap.UpdateAdUser(adUser);
                ldapOperationResult.operationSuccessful = true;

                return ldapOperationResult;
            }
        }

        /// <summary>
        /// Add the user in AD to group(s).
        /// </summary>
        /// <param name="ldapConnectionInfo"></param>
        /// <param name="adUser"></param>
        /// <param name="otherData"></param>
        /// <returns></returns>
        public static Output AD_AddGroups(LdapConnectionInfo ldapConnectionInfo, AdUser adUser, Options otherData)
        {
            var ldapOperationResult = new Output { operationSuccessful = false, user = null };

            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                ldapOperationResult.operationSuccessful = ldap.AddAdUserToGroup(adUser, otherData.groups);

                return ldapOperationResult;
            }
        }

        public static Output WindowsUser_Create(LdapConnectionInfo ldapConnectionInfo, WindowsUser windowsUser)
        {
            var ldapOperationResult = new Output { operationSuccessful = false, user = null };

            using (var ldapService = new LdapService(ldapConnectionInfo))
            {
                ldapOperationResult.user = ldapService.CreateWindowsUser(windowsUser);
                ldapOperationResult.operationSuccessful = true;

                return ldapOperationResult;
            }
        }

        public static Output WindowsUser_Update(LdapConnectionInfo ldapConnectionInfo, WindowsUser user)
        {
            var ldapOperationResult = new Output { operationSuccessful = false, user = null };

            using (var ldapService = new LdapService(ldapConnectionInfo))
            {
                ldapOperationResult.user = ldapService.UpdateWindowsUser(user);
                ldapOperationResult.operationSuccessful = true;

                return ldapOperationResult;
            }
        }

        public static Output WindowsUser_AddToGroup(LdapConnectionInfo ldapConnectionInfo, WindowsUser user, Options otherData)
        {
            var ldapOperationResult = new Output { operationSuccessful = false, user = null };

            using (var ldapService = new LdapService(ldapConnectionInfo))
            {
                ldapOperationResult.operationSuccessful = ldapService.AddUserToGroup(user, otherData.groups);

                return ldapOperationResult;
            }
        }


    }
}
