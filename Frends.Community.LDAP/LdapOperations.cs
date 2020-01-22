using Frends.Community.LDAP.Models;
using Frends.Community.LDAP.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;

#pragma warning disable CS1591

namespace Frends.Community.LDAP
{
    
    public static class LdapActiveDirectoryOperations 
    {
        /// <summary>
        /// Searches Active Directory for objects specified by the given Path + filter, included in the AD_FetchObjectProperties class.
        /// </summary>
        /// <param name="ldapConnectionInfo">The LDAP connection information</param>
        /// <param name="SearchParameters">Path and filter needed for the query</param>
        /// <returns>LdapResult class: the Collection of the DirectoryEntry classes.</returns>
        public static List<OutputObjectEntry> AD_FetchObjects([PropertyTab] LdapConnectionInfo ldapConnectionInfo, [PropertyTab] AD_FetchObjectProperties SearchParameters)
        {

            var ret_outputs = new List<OutputObjectEntry>(); 
            List<DirectoryEntry> tmpObjectEntries;

            ldapConnectionInfo.LdapUri = ldapConnectionInfo.LdapUri + "/"+SearchParameters.Path;

            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                tmpObjectEntries = ldap.SearchObjectsByFilter(SearchParameters.Filter);
            }

            foreach (var item in tmpObjectEntries)
            {
                OutputObjectEntry output_class = new OutputObjectEntry();
                output_class.ObjectEntry = item;
                ret_outputs.Add(output_class);
            }
            return ret_outputs;
        }

        /// <summary>
        /// Create a user to AD.
        /// </summary>
        /// <param name="ldapConnectionInfo">The LDAP connection information</param>
        /// <param name="adUser">The user record to be created</param>
        /// <param name="Password">Passes two parameters to this task: bool setPassword, which defines if a password should be set at create time, and string newPassword, containing the password to be set.</param>
        /// <returns>LdapResult class, which carries a copy of the created user record.</returns>
        public static OutputUser AD_CreateUser([PropertyTab] LdapConnectionInfo ldapConnectionInfo, [PropertyTab] CreateADuser adUser, AD_CreateUserProperties Password )
        {
            var ldapOperationResult = new OutputUser { OperationSuccessful = false, User = null };

            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                ldapOperationResult.User = ldap.CreateAdUser(adUser);

                if (Password.SetPassword) 
                {
                    SetPassword.SetUserPassword(ldapConnectionInfo.LdapUri,adUser.Path,ldapConnectionInfo.Username,ldapConnectionInfo.Password,adUser.CN, Password.NewPassword);
                }

                ldapOperationResult.OperationSuccessful = true;

                return ldapOperationResult;
            }   
        }

        /// <summary>
        /// Update a user in the AD.
        /// </summary>
        /// <param name="ldapConnectionInfo">The LDAP connection information</param>
        /// <param name="adUser">The user record to be updated</param>
        /// <returns>LdapResult class, which carries a copy of the updated user record.</returns>
        public static OutputUser AD_UpdateUser([PropertyTab] LdapConnectionInfo ldapConnectionInfo, [PropertyTab] UpdateADuser adUser)
        {
            var ldapOperationResult = new OutputUser { OperationSuccessful = false, User = null };

            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                ldapOperationResult.User = ldap.UpdateAdUser(adUser);
                ldapOperationResult.OperationSuccessful = true;

                return ldapOperationResult;
            }
        }

        /// <summary>
        /// Add the user in AD to group(s).
        /// </summary>
        /// <param name="ldapConnectionInfo"></param>
        /// <param name="User"></param>
        /// <param name="GroupsToAdd"></param>
        /// <returns></returns>
        public static Output AD_AddGroups([PropertyTab] LdapConnectionInfo ldapConnectionInfo, [PropertyTab] AD_AddGroupsUserProperties User, [PropertyTab] AD_AddGroupsProperties GroupsToAdd)
        {
            var ldapOperationResult = new Output { OperationSuccessful = false };
 
            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                ldapOperationResult.OperationSuccessful = ldap.AddAdUserToGroup(User.Dn, GroupsToAdd.Groups);

                return ldapOperationResult;
            }
        }

        /// <summary>
        /// Delete AD user.
        /// </summary>
        /// <param name="ldapConnectionInfo">Properties to define LDAP connection</param>
        /// <param name="userProperties">Properties to define the user to be deleted</param>
        /// <returns>operationSuccessful = true if operation is ok.</returns>
        public static Output AD_DeleteUser([PropertyTab] LdapConnectionInfo ldapConnectionInfo, [PropertyTab] AD_DeleteUserProperties userProperties)
        {
            var ret_output = new Output();
            List<DirectoryEntry> tmpObjectEntries;
            ret_output.OperationSuccessful = false;

            ldapConnectionInfo.LdapUri = ldapConnectionInfo.LdapUri + "/" + userProperties.Path;

            string filter = "(&(objectClass=user)(cn=" + userProperties.Cn + "))";

            using (var ldap = new LdapService(ldapConnectionInfo))// @"(&(objectClass=user)(cn=MattiMeikalainen))
            {
                tmpObjectEntries = ldap.SearchObjectsByFilter(filter);
                if (tmpObjectEntries.Count > 0)
                {
                    ldap.DeleteAdUser(tmpObjectEntries[0]);
                }
                else
                {
                    throw new System.Exception($"Did not find any entries matching filter {filter} from {ldapConnectionInfo.LdapUri}");
                }
            }

            ret_output.OperationSuccessful = true;
            return ret_output;
        }

        /// <summary>
        /// Rename AD user.
        /// </summary>
        /// <param name="ldapConnectionInfo">Properties to define LDAP connection</param>
        /// <param name="userProperties">Properties to define the user to be renamed</param>
        /// <returns>operationSuccessful = true if operation is ok.</returns>
        public static OutputUser AD_RenameUser([PropertyTab] LdapConnectionInfo ldapConnectionInfo, [PropertyTab] AD_RenameUserProperties userProperties)
        {
            var ldapOperationResult = new OutputUser { OperationSuccessful = false, User = null };

            List<DirectoryEntry> tmpObjectEntries;
            
            ldapConnectionInfo.LdapUri = ldapConnectionInfo.LdapUri + "/" + userProperties.Path;

            string filter = "(&(objectClass=user)(cn=" + userProperties.Cn + "))";

            using (var ldap = new LdapService(ldapConnectionInfo)) // @"(&(objectClass=user)(cn=MattiMeikalainen))
            {
                tmpObjectEntries = ldap.SearchObjectsByFilter(filter);
                if (tmpObjectEntries.Count == 1)
                {
                    ldapOperationResult.User = ldap.RenameAdUser(tmpObjectEntries[0], userProperties.NewCn);
                }
                else if (tmpObjectEntries.Count == 0)
                {
                    throw new System.Exception($"Did not find any entries matching filter {filter} from {ldapConnectionInfo.LdapUri}");
                }
                else if (tmpObjectEntries.Count > 1)
                {
                    throw new System.Exception($"Found more than one entry matching filter {filter} from {ldapConnectionInfo.LdapUri}");
                }
            }
            
            ldapOperationResult.OperationSuccessful = true;
            return ldapOperationResult;
        }

        /// <summary>
        /// Remove AD object from a set of groups
        /// </summary>
        /// <param name="ldapConnectionInfo"></param>
        /// <param name="target"></param>
        /// <param name="groupsToRemoveFrom"></param>
        /// <returns>Object { bool operationSuccessful }</returns>
        public static Output AD_RemoveFromGroups([PropertyTab] LdapConnectionInfo ldapConnectionInfo, [PropertyTab] AD_RemoveFromGroupsTargetProperties target, [PropertyTab] AD_RemoveFromGroupsGroupProperties groupsToRemoveFrom)
        {
            var result = new Output();

            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                result.OperationSuccessful = ldap.RemoveFromGroups(target.Dn, groupsToRemoveFrom.Groups);
            }

            return result;
        }
    }
}
