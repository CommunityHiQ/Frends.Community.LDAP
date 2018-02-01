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
using Frends.Tasks.Attributes;

#pragma warning disable CS1591

namespace Frends.Community.LDAP
{
    /// <summary>
    /// Properties for User search
    /// If the operation returns a user entry, it is returned.
    /// </summary>
    public class AD_UserExistsProperties
    {
        /// <summary>
        ///  The name of the attribute to search by.
        /// </summary>
        /// 
        public string attribute { set; get; }
        /// <summary>
        /// The value of the attribute to search by.
        /// </summary>
        public string value { set; get; }
    }

    /// <summary>
    /// Properties for create user
    /// </summary>
    public class AD_CreateUserProperties
    {
        /// <summary>
        ///  Defines if password should be set at create time.
        /// </summary>
        /// 
        public bool setPassword { set; get; }
        /// <summary>
        /// AD Create user: Defines the new password if needed.
        /// </summary>
        [PasswordPropertyText(true)]
        public string newPassword { set; get; }
    }

    /// <summary>
    /// Properties for AD groups
    /// </summary>
    public class AD_AddGroupsProperties
    {
        /// <summary>
        ///  To which groups the user should be added.
        /// </summary>
        /// 
        public string[] groups { set; get; }
    }

    /// <summary>
    /// Result class.
    /// 
    /// Fields and their descriptions:
    /// 
    /// - operationSuccessful: Tells if the requested operation was performed successfully.
    /// - user: Returns a user entry.
    /// </summary>
    public class OutputUser
    {
        public bool operationSuccessful { get; set; }
        public DirectoryEntry user { get; set; }

        public object GetUserProperty(string Attribute)
        {
            return user.Properties[Attribute];
        }
    }

    /// <summary>
    /// Result class.
    /// - operationSuccessful: Tells if the requested operation was performed successfully.
    /// </summary>
    public class Output
    {
        public bool operationSuccessful { get; set; }
    }
    public static class LdapOperations 
    {
        /// <summary>
        /// Searches Active Directory for user(s) specified by the given attribute and its value, included in the AD_UserExistsProperties class.
        /// </summary>
        /// <param name="ldapConnectionInfo">The LDAP connection information</param>
        /// <param name="SearchParameters">Other data needed for the query</param>
        /// <returns>LdapResult class: bool OperationSuccessful, DirectoryEntry user</returns>
        public static OutputUser AD_UserExists([CustomDisplay(DisplayOption.Tab)] LdapConnectionInfo ldapConnectionInfo, [CustomDisplay(DisplayOption.Tab)] AD_UserExistsProperties SearchParameters)
        {
            var ldapOperationResult = new OutputUser { operationSuccessful = false, user = null };

            DirectoryEntry user;
            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                user = ldap.SearchUser(SearchParameters.attribute, SearchParameters.value);
            }

            ldapOperationResult.operationSuccessful = !(user == null);

            if (ldapOperationResult.operationSuccessful == true) ldapOperationResult.user = user;

            return ldapOperationResult;
        }

        /// <summary>
        /// Create a user to AD.
        /// </summary>
        /// <param name="ldapConnectionInfo">The LDAP connection information</param>
        /// <param name="adUser">The user record to be created</param>
        /// <param name="Password">Passes two parameters to this task: bool setPassword, which defines if a password should be set at create time, and string newPassword, containing the password to be set.</param>
        /// <returns>LdapResult class, which carries a copy of the created user record.</returns>
        public static OutputUser AD_CreateUser([CustomDisplay(DisplayOption.Tab)] LdapConnectionInfo ldapConnectionInfo, [CustomDisplay(DisplayOption.Tab)] AdUser adUser, AD_CreateUserProperties Password )
        {
            var ldapOperationResult = new OutputUser { operationSuccessful = false, user = null };

            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                ldapOperationResult.user = ldap.CreateAdUser(adUser);

                if (Password.setPassword) 
                {
                    SetPassword.SetUserPassword(ldapConnectionInfo.LdapUri,adUser.OU,ldapConnectionInfo.Username,ldapConnectionInfo.Password,adUser.CN, Password.newPassword);
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
        public static OutputUser AD_UpdateUser([CustomDisplay(DisplayOption.Tab)] LdapConnectionInfo ldapConnectionInfo, [CustomDisplay(DisplayOption.Tab)] AdUser adUser)
        {
            var ldapOperationResult = new OutputUser { operationSuccessful = false, user = null };

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
        /// <param name="GroupsToAdd"></param>
        /// <returns></returns>
        public static Output AD_AddGroups([CustomDisplay(DisplayOption.Tab)] LdapConnectionInfo ldapConnectionInfo, [CustomDisplay(DisplayOption.Tab)] AdUser adUser, AD_AddGroupsProperties GroupsToAdd)
        {
            var ldapOperationResult = new Output { operationSuccessful = false };

            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                ldapOperationResult.operationSuccessful = ldap.AddAdUserToGroup(adUser, GroupsToAdd.groups);

                return ldapOperationResult;
            }
        }
    }
}
