using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices;

#pragma warning disable CS1591

namespace Frends.Community.LDAP
{
    /// <summary>
    /// Properties for AD's object search
    /// </summary>
    public class AD_FetchObjectProperties
    {
        /// <summary>
        /// Defines path which used to search object(s)
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("CN=Users,DC=FRENDSTest01,DC=net")]
        public string Path { get; set; }

        /// <summary>
        /// Defines filter which is used to search object(s)
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
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
        ///  To which groups the user should be added(For example. CN=Guests,CN=Builtin).
        /// </summary>
        public string[] groups { set; get; }
    }

    /// <summary>
    ///  User to be added into groups
    /// </summary>
    public class AD_AddGroupsUserProperties
    {
        [DefaultValue("CN=UserName,CN=Users,DC=FRENDSTest01,DC=net")]
        [DisplayFormat(DataFormatString = "Text")]
        public string dn { set; get; }
    }

    /// <summary>
    /// Properties for AD delete user
    /// </summary>
    public class AD_DeleteUserProperties
    {
        /// <summary>
        /// Path to remove from
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("OU=Users,DC=FRENDSTest01,DC=net")]
        public string Path { get; set; }

        /// <summary>
        ///  Common name of the user
        /// </summary>
        [DefaultValue("UserName")]
        [DisplayFormat(DataFormatString = "Text")]
        public string Cn { set; get; }
    }

    public class AD_RemoveFromGroupsTargetProperties
    {
        /// <summary>
        /// Distinguished name of the object to remove from groups
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
        public object[] GetProperty(String Attribute)// int32, string, ...
        {
            var object_type = ObjectEntry.Properties[Attribute].Value;

            if (object_type is System.Object[]) // many objects found
            {
                return (Object[])ObjectEntry.Properties[Attribute].Value;
            }
            else // just one object found
            {
                object[] ret = new object[] { ObjectEntry.Properties[Attribute].Value };
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
}
