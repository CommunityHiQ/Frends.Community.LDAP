using Frends.Community.LDAP.Models;
using Frends.Community.LDAP.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

#pragma warning disable CS1591

namespace Frends.Community.LDAP
{
    
    public static class LdapActiveDirectoryOperations 
    {

        /// <summary>
        /// Searches Active Directory for objects specified by the given Path + filter, included in the AD_SearchObjectProperties class.
        /// </summary>
        /// <param name="ldapConnectionInfo">The LDAP connection information</param>
        /// <param name="SearchParameters">Path, filter and returned properties needed for the query</param>
        /// <returns>LdapResult class: the Collection of the SearchEntry classes.</returns>
        public static List<OutputSearchEntry> AD_SearchObjects([PropertyTab] LdapConnectionInfo ldapConnectionInfo, [PropertyTab] AD_SearchObjectProperties SearchParameters)
        {

            var ldapQueryInfo = new LdapConnectionInfo
            {
                LdapUri = ldapConnectionInfo.LdapUri + "/" + SearchParameters.Path,
                Username = ldapConnectionInfo.Username,
                Password = ldapConnectionInfo.Password
            };

            List<SearchResult> tmpObjectEntries;

            // Search objects.
            using (var ldap = new LdapService(ldapQueryInfo))
                 tmpObjectEntries = ldap.SearchObjectsByFilterSpecifyProperties(SearchParameters.Filter, SearchParameters.PropertiesToLoad, SearchParameters.PageSize);

            // Create & return result list.
            var retOutputs = new List<OutputSearchEntry>();

            foreach (var item in tmpObjectEntries)
            {
                var outputClass = new OutputSearchEntry
                {
                    SearchEntry = item
                };
                retOutputs.Add(outputClass);
            }
            return retOutputs;
        }

        /// <summary>
        /// Searches Active Directory for objects specified by the given Path + filter, included in the AD_FetchObjectProperties class.
        /// </summary>
        /// <param name="ldapConnectionInfo">The LDAP connection information</param>
        /// <param name="SearchParameters">Path and filter needed for the query</param>
        /// <returns>LdapResult class: the Collection of the DirectoryEntry classes.</returns>
        public static List<OutputObjectEntry> AD_FetchObjects([PropertyTab] LdapConnectionInfo ldapConnectionInfo, [PropertyTab] AD_FetchObjectProperties SearchParameters)
        {

            var retOutputs = new List<OutputObjectEntry>(); 
            List<DirectoryEntry> tmpObjectEntries;

            ldapConnectionInfo.LdapUri = ldapConnectionInfo.LdapUri + "/"+SearchParameters.Path;

            using (var ldap = new LdapService(ldapConnectionInfo)) tmpObjectEntries = ldap.SearchObjectsByFilter(SearchParameters.Filter);

            foreach (var item in tmpObjectEntries)
            {
                var outputClass = new OutputObjectEntry
                {
                    ObjectEntry = item
                };
                retOutputs.Add(outputClass);
            }
            return retOutputs;
        }

        /// <summary>
        /// Create a user to AD. The task AD_SetUserPassword is meant as a replacement for setting the password in conjunction with AD user creation.
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
                    SetPassword.SetUserPassword(ldapConnectionInfo.LdapUri,adUser.Path,ldapConnectionInfo.Username,ldapConnectionInfo.Password,adUser.CN, Password.NewPassword);

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
            var retOutput = new Output
            {
                OperationSuccessful = false
            };

            ldapConnectionInfo.LdapUri = ldapConnectionInfo.LdapUri + "/" + userProperties.Path;

            var filter = "(&(objectClass=user)(cn=" + userProperties.Cn + "))";

            // @"(&(objectClass=user)(cn=MattiMeikalainen))
            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                var tmpObjectEntries = ldap.SearchObjectsByFilter(filter);
                if (tmpObjectEntries.Count > 0) ldap.DeleteAdUser(tmpObjectEntries[0]);
                else throw new Exception($"Did not find any entries matching filter {filter} from {ldapConnectionInfo.LdapUri}");
            }

            retOutput.OperationSuccessful = true;
            return retOutput;
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

            var filter = "(&(objectClass=user)(cn=" + userProperties.Cn + "))";

            // @"(&(objectClass=user)(cn=MattiMeikalainen))
            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                tmpObjectEntries = ldap.SearchObjectsByFilter(filter);
                if (tmpObjectEntries.Count == 1) ldapOperationResult.User = ldap.RenameAdUser(tmpObjectEntries[0], userProperties.NewCn);
                else if (tmpObjectEntries.Count == 0) throw new Exception($"Did not find any entries matching filter {filter} from {ldapConnectionInfo.LdapUri}");
                else if (tmpObjectEntries.Count > 1) throw new Exception($"Found more than one entry matching filter {filter} from {ldapConnectionInfo.LdapUri}");
            }
            
            ldapOperationResult.OperationSuccessful = true;
            return ldapOperationResult;
        }

        /// <summary>
        /// Remove AD object from a set of groups.
        /// </summary>
        /// <param name="ldapConnectionInfo"></param>
        /// <param name="target"></param>
        /// <param name="groupsToRemoveFrom"></param>
        /// <returns>Object { bool operationSuccessful }</returns>
        public static Output AD_RemoveFromGroups([PropertyTab] LdapConnectionInfo ldapConnectionInfo, [PropertyTab] AD_RemoveFromGroupsTargetProperties target, [PropertyTab] AD_RemoveFromGroupsGroupProperties groupsToRemoveFrom)
        {
            var result = new Output();

            using (var ldap = new LdapService(ldapConnectionInfo)) result.OperationSuccessful = ldap.RemoveFromGroups(target.Dn, groupsToRemoveFrom.Groups);

            return result;
        }


        /// <summary>
        /// Move a object to another OU (Organizational Unit). Returns class, which carries a copy of the updated object.
        /// </summary>
        /// <param name="ldapConnectionInfo"></param>
        /// <param name="adObject"></param>
        /// <returns>Object { DirectoryEntry ObjectEntryCopy }</returns>
        public static MoveAdObjectResult AD_MoveObject([PropertyTab] LdapConnectionInfo ldapConnectionInfo, [PropertyTab] MoveObject adObject)
        {
            var result = new MoveAdObjectResult
            {
                OperationSuccessful = false
            };

            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                result.ObjectEntryCopy = ldap.MoveAdObject(adObject);
                result.OperationSuccessful = true;
            }

            return result;
        }


        /// <summary>
        /// Sets password for user in AD.
        /// This task allows the use of other ways of binding to the server than simple bind, which is the one that is used when setting the password in AD_CreateUser.
        /// </summary>
        /// <param name="passwordParameters">Input parameters and options</param>
        /// <returns>Object { bool OperationSuccessful, string UserPrincipalName, string LogString }</returns>
        public static PasswordOutput AD_SetUserPassword([PropertyTab] PasswordParameters passwordParameters)
        {
            var result = new PasswordOutput { OperationSuccessful = false, UserPrincipalName = null, LogString = null };
            PrincipalContext pContext = null;

            var serverName = passwordParameters.AdServer.ToLower().Replace("ldap://", "").Replace("ldaps://", "");
            var userPN = passwordParameters.UserPrincipalName;

            try
            {
                result.LogString += "Attempting to connect to server.";

                // Create context.
                pContext = new PrincipalContext(ContextType.Domain, serverName, passwordParameters.AdContainer, passwordParameters.GetContextType(), passwordParameters.Username, passwordParameters.Password);
                result.LogString += "Context created and connection formed. Server: " + pContext.ConnectedServer + " Container: " +
                   pContext.Container + " Context type: " + pContext.ContextType + " UserName: " + pContext.UserName + ";";

                // Fetch the principal object for the user.
                var user = UserPrincipal.FindByIdentity(pContext, IdentityType.UserPrincipalName, userPN);
                if (user == null)
                {
                    result.LogString += "User " + userPN + " not found.";
                    throw new System.ArgumentNullException();
                }
                else
                {
                    result.LogString += "User found: " + user.DistinguishedName + ";";

                    // Set user password.
                    user.SetPassword(passwordParameters.NewPassword);
                    result.LogString += "Password set;";

                    // Save the changes to the store.
                    user.Save();
                    result.LogString += "User saved;";

                    // Finalize result.
                    result.OperationSuccessful = true;
                    result.UserPrincipalName = passwordParameters.UserPrincipalName;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Password could not be set. Log: " + result.LogString, ex);
            }
            finally
            {
                pContext?.Dispose();
            }

            return result;
        }
    }
}
