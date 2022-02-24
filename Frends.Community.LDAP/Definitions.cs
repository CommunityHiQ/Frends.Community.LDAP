using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices;

#pragma warning disable CS1591

namespace Frends.Community.LDAP
{

    /// <summary>
    /// Properties for AD's object search.
    /// </summary>
    public class AD_SearchObjectProperties
    {
        /// <summary>
        /// Defines path which used to search object(s).
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("CN=Users,DC=FRENDSTest01,DC=net")]
        public string Path { get; set; }

        /// <summary>
        /// Defines filter which is used to search object(s).
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("(&(objectClass=user)(sAMAccountName=TestAdmin))")]
        public string Filter { set; get; }

        /// <summary>
        /// Defines properties to load.
        /// Empty list fetch all properties.
        /// adspath is returned automatically.
        /// </summary>
        public string[] PropertiesToLoad { get; set; }

        /// <summary>
        /// Defines PageSize when fetch big amounts of data from server. 
        /// 0 - not used. Server limits returned objects count.
        /// 1000 - a safe first value. Client fetched 1000 objects per internal call. 
        /// </summary>
        [DefaultValue(0)]
        public int PageSize { get; set; }
    }


    /// <summary>
    /// Properties for AD's object search.
    /// </summary>
    public class AD_FetchObjectProperties
    {
        /// <summary>
        /// Defines path which used to search object(s).
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("CN=Users,DC=FRENDSTest01,DC=net")]
        public string Path { get; set; }

        /// <summary>
        /// Defines filter which is used to search object(s).
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("(&(objectClass=user)(sAMAccountName=TestAdmin))")]
        public string Filter { set; get; }
    }

    /// <summary>
    /// Properties for create user.
    /// </summary>
    public class AD_CreateUserProperties
    {
        /// <summary>
        ///  Defines if password should be set at create time.
        /// </summary>
        public bool SetPassword { set; get; }
        /// <summary>
        /// AD Create user: Defines the new password if needed.
        /// </summary>
        [PasswordPropertyText(true)]
        public string NewPassword { set; get; }
    }

    /// <summary>
    /// Properties for AD groups.
    /// </summary>
    public class AD_AddGroupsProperties
    {
        /// <summary>
        /// To which groups the user should be added(For example. CN=Guests,CN=Builtin).
        /// </summary>
        public string[] Groups { set; get; }
    }

    /// <summary>
    ///  User to be added into groups.
    /// </summary>
    public class AD_AddGroupsUserProperties
    {
        [DefaultValue("CN=UserName,CN=Users,DC=FRENDSTest01,DC=net")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Dn { set; get; }
    }

    /// <summary>
    /// Properties for AD delete user.
    /// </summary>
    public class AD_DeleteUserProperties
    {
        /// <summary>
        /// Path to remove from.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("OU=Users,DC=FRENDSTest01,DC=net")]
        public string Path { get; set; }

        /// <summary>
        /// Common name of the user.
        /// </summary>
        [DefaultValue("UserName")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Cn { set; get; }
    }

    /// <summary>
    /// Properties for AD rename user.
    /// </summary>
    public class AD_RenameUserProperties
    {
        /// <summary>
        /// Path to the user to be renamed.
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("OU=Users,DC=FRENDSTest01,DC=net")]
        public string Path { get; set; }

        /// <summary>
        /// Current common name of the user.
        /// </summary>
        [DefaultValue("UserName")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Cn { set; get; }

        /// <summary>
        /// New name for the user.
        /// </summary>
        [DefaultValue("NewUserName")]
        [DisplayFormat(DataFormatString = "Text")]
        public string NewCn { set; get; }
    }

    public class AD_RemoveFromGroupsTargetProperties
    {
        /// <summary>
        /// Distinguished name of the object to remove from groups.
        /// </summary>
        [DefaultValue("CN=UserName,CN=Users,DC=FRENDSTest01,DC=net")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Dn { set; get; }
    }

    public class AD_RemoveFromGroupsGroupProperties
    {
        /// <summary>
        /// Groups to remove the object from (For example. CN=Guests,CN=Builtin).
        /// </summary>
        public string[] Groups { set; get; }
    }

    /// <summary>
    /// Result class for object search.
    /// Fields and their descriptions:
    /// - SearchEntry is search's result data(SearchResult)
    /// Methods:
    /// - GetPropertyStringValue(name) : returns property's first value.
    /// </summary>
    public class OutputSearchEntry
    {
        public SearchResult SearchEntry { get; set; }

        /// <summary>
        /// Returns property's first value as string or null.
        /// </summary>
        /// <param name="name">Propterty name</param>
        /// <returns>Property's first value or null if property not exists.</returns>
        public string GetPropertyStringValue(string name)
        {
            if (SearchEntry.Properties.Contains(name))
            {
                if (SearchEntry.Properties[name].Count > 0) return SearchEntry.Properties[name][0].ToString();
            }
            return null;
        }
    }

    /// <summary>
    /// Result class.
    /// Fields and their descriptions:
    /// - ObjectEntry is object's entry data(DirectoryEntry)
    /// </summary>
    public class OutputObjectEntry
    {
        public DirectoryEntry ObjectEntry { get; set; }

        // Int64.
        public object GetPropertyLargeInteger(string attribute)
        {
            List<object> ret = new List<object>();

            var objectType = ObjectEntry.Properties[attribute].Value;

            // Many objects found.
            if (objectType is object[])
            {
                foreach (var _ in (object[])ObjectEntry.Properties[attribute].Value) ret.Add(ProcessLargeInteger(attribute));
                return ret;
            }

            // Just one object found.
            else return ProcessLargeInteger(attribute);
        }
            
        private long ProcessLargeInteger(string attribute)
        {
            var adsLargeInteger = ObjectEntry.Properties[attribute].Value;

            if (adsLargeInteger == null) throw new ArgumentException("User attribute not found", attribute);

            var highPart = (int)adsLargeInteger.GetType().InvokeMember("HighPart", System.Reflection.BindingFlags.GetProperty, null, adsLargeInteger, null);
            var lowPart = (int)adsLargeInteger.GetType().InvokeMember("LowPart", System.Reflection.BindingFlags.GetProperty, null, adsLargeInteger, null);

            // Compensate for IADsLargeInteger interface bug.
            if (lowPart < 0) highPart += 1;
            return highPart * ((long)uint.MaxValue + 1) + lowPart;
        }



        // GetProperty returns collection even if there are one object match.
        // Int32, string, ...
        public object[] GetProperty(string attribute)
        {
            var objectType = ObjectEntry.Properties[attribute].Value;

            // Many objects found.
            if (objectType is object[]) return (object[])ObjectEntry.Properties[attribute].Value;

            // Just one object found.
            else return new [] {ObjectEntry.Properties[attribute].Value};
        }

        public DateTime GetAccountExpiresDateTime()
        {
            var largeIntObject = GetPropertyLargeInteger("accountExpires");

            // 0x7FFFFFFFFFFFFFFF = account never expires -> doesn't fit in DateTime.
            // Return DateTime.MaxValue instead.
            if ((long)largeIntObject > DateTime.MaxValue.Ticks) return DateTime.MaxValue;
            else return DateTime.FromFileTime(((long)largeIntObject));
        }

        public DateTime GetPropertyDateTime(string attribute)
        {

            try
            {
                var largeIntObject = GetPropertyLargeInteger((string)attribute);

                if ((long)largeIntObject > DateTime.MaxValue.Ticks) return DateTime.MaxValue;
                else return DateTime.FromFileTime((long)largeIntObject);

            }
            catch (Exception ex)
            {
                throw new ArgumentException("Only numeric values can be fetch.", ex);
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
        public bool OperationSuccessful { get; set; }
        public DirectoryEntry User { get; set; }

        public object GetUserProperty(string Attribute)
        {
            return User.Properties[Attribute].Value.ToString();
        }
    }

    /// <summary>
    /// Result class.
    /// - operationSuccessful: Tells if the requested operation was performed successfully.
    /// </summary>
    public class Output
    {
        public bool OperationSuccessful { get; set; }
    }


    /// <summary>
    /// Result class.
    /// - ObjectEntry is copy of moved object entry data.
    /// - operationSuccessful: Tells if the requested operation was performed successfully.
    /// </summary>
    public class MoveAdObjectResult
    {

        public DirectoryEntry ObjectEntryCopy { get; set; }
        public bool OperationSuccessful { get; set; }

    }

    /// <summary>
    /// Result class.
    /// Fields and their descriptions:
    /// - OperationSuccessful: Tells if the requested operation was performed successfully.
    /// - UserPrincipalName: The userPrincipalName of the affected user.
    /// - LogString: Log of operation.
    /// </summary>
    public class PasswordOutput
    {
        public bool OperationSuccessful { get; set; }
        public string UserPrincipalName { get; set; }
        public string LogString { get; set; }
    }
}
