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
    /// Properties for AD's object search
    /// </summary>
    public class AD_FetchObjectProperties
    {
        /// <summary>
        /// Defines filter which is used to search object(s)
        /// </summary>
        [DefaultValue("(&(objectClass=user)(sAMAccountName=TestAdmin))")]
        public string filter { set; get; }
    }

    /// <summary>
    /// Properties for create user
    /// </summary>
    public class AD_CreateUserProperties
    {
        /// <summary>
        ///  Defines if password should be set at create time.
        /// </summary>
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
        public string[] groups { set; get; }
    }

    /// <summary>
    /// Result class.
    /// Fields and their descriptions:
    /// - ObjectEntry is object's entry data(DirectoryEntry)
    /// </summary>
    public class OutputObjectEntry
    {
        public DirectoryEntry ObjectEntry { get; set; }

        public object GetPropertyLargeInteger(string Attribute)// int64
        {
            List<object> ret = new List<object>();
            var object_type = ObjectEntry.Properties[Attribute].Value;

            if (object_type is System.Object[]) // many objects found
            {
                foreach (var item in (Object[])(ObjectEntry.Properties[Attribute].Value))
                {                          
                    var adsLargeInteger = ObjectEntry.Properties[Attribute].Value;
                    var highPart = (Int32)adsLargeInteger.GetType().InvokeMember("HighPart", System.Reflection.BindingFlags.GetProperty, null, adsLargeInteger, null);
                    var lowPart = (Int32)adsLargeInteger.GetType().InvokeMember("LowPart", System.Reflection.BindingFlags.GetProperty, null, adsLargeInteger, null);
                    var recipientType = highPart * ((Int64)UInt32.MaxValue + 1) + lowPart;
                    ret.Add(recipientType);
                }
                return ret;
            }
            else // just one object found
            {               
                var adsLargeInteger = ObjectEntry.Properties[Attribute].Value;
                var highPart = (Int32)adsLargeInteger.GetType().InvokeMember("HighPart", System.Reflection.BindingFlags.GetProperty, null, adsLargeInteger, null);
                var lowPart = (Int32)adsLargeInteger.GetType().InvokeMember("LowPart", System.Reflection.BindingFlags.GetProperty, null, adsLargeInteger, null);
                var recipientType = highPart * ((Int64)UInt32.MaxValue + 1) + lowPart;
                ret.Add(recipientType);
                return recipientType;
            }
        }

        // GetProperty returns collection even if there are one object match.
        public object GetProperty(String Attribute)// int32, string, ...
        {
            var object_type = ObjectEntry.Properties[Attribute].Value;

            if(object_type is System.Object[]) // many objects found
            {
                return ObjectEntry.Properties[Attribute].Value;
            }
            else // just one object found
            {
                List<object> ret = new List<object>();
                ret.Add(ObjectEntry.Properties[Attribute].Value);
                return ret;
            }
        }
    }

    /// <summary>
    /// Result class.
    /// Fields and their descriptions:
    /// - operationSuccessful: Tells if the requested operation was performed successfully.
    /// - user: Returns a user entry.
    /// </summary>
    public class OutputUser
    {
        public bool operationSuccessful { get; set; }
        public DirectoryEntry user { get; set; }

        public object GetUserProperty(string Attribute)
        {
            return user.Properties[Attribute].Value.ToString();
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
    public static class LdapActiveDirectoryOperations 
    {
        /// <summary>
        /// Searches Active Directory for objects specified by the given filter, included in the AD_FetchObjectProperties class.
        /// </summary>
        /// <param name="ldapConnectionInfo">The LDAP connection information</param>
        /// <param name="SearchParameters">Filter needed for the query</param>
        /// <returns>LdapResult class: the Collection of the DirectoryEntry classes.</returns>
        public static List<OutputObjectEntry> AD_FetchObjects([CustomDisplay(DisplayOption.Tab)] LdapConnectionInfo ldapConnectionInfo, [CustomDisplay(DisplayOption.Tab)] AD_FetchObjectProperties SearchParameters)
        {

            var ret_outputs = new List<OutputObjectEntry>(); 
            List<DirectoryEntry> tmpObjectEntries;

            using (var ldap = new LdapService(ldapConnectionInfo))
            {
                tmpObjectEntries = ldap.SearchObjectsByFilter(SearchParameters.filter);
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

        private static long  ConvertToLargeInteger(object value)
        {
            var adsLargeInteger = value;
            var highPart = (Int32)adsLargeInteger.GetType().InvokeMember("HighPart", System.Reflection.BindingFlags.GetProperty, null, adsLargeInteger, null);
            var lowPart = (Int32)adsLargeInteger.GetType().InvokeMember("LowPart", System.Reflection.BindingFlags.GetProperty, null, adsLargeInteger, null);
            var recipientType = highPart * ((Int64)UInt32.MaxValue + 1) + lowPart;
            return recipientType;
        }
    }
}
