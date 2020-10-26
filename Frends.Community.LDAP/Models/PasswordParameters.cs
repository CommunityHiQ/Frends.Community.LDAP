using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices.AccountManagement;

#pragma warning disable CS1591

namespace Frends.Community.LDAP.Models
{
    public class PasswordParameters
    {
        /// <summary>
        /// The name or ip address of the AD server
        /// </summary>
        [DefaultValue("xxx.xxx.xxx")]
        [DisplayFormat(DataFormatString = "Text")]
        public string AdServer { get; set; }
        /// <summary>
        /// AD Container
        /// </summary>
        [DefaultValue("DC=Test")]
        [DisplayFormat(DataFormatString = "Text")]
        public string AdContainer { get; set; }
        /// <summary>
        /// Username for AD server
        /// </summary>
        [DisplayFormat(DataFormatString = "Text")]
        public string Username { get; set; }
        /// <summary>
        /// Password for AD server
        /// </summary>
        [PasswordPropertyText(true)]
        public string Password { get; set; }
        /// <summary>
        /// The userPrincipalName of the user whose password is to be set
        /// </summary>
        [DefaultValue("test.person@test.com")]
        [DisplayFormat(DataFormatString = "Text")]
        public string UserPrincipalName { get; set; }
        /// <summary>
        /// The user's new password
        /// </summary>
        [PasswordPropertyText(true)]
        public string NewPassword { get; set; }
        /// <summary>
        /// Default value is Simple Bind
        /// </summary>
        public Context[] ContextOptionFlags { get; set; }

        internal ContextOptions GetContextType()
        {
            // ContextOption does not include value None / 0, so we need to default to something
            // and remove it afterwards if it was not specified
            var contextOption = ContextOptions.SimpleBind;
            var sbNeeded = false;
            foreach (var contOpt in ContextOptionFlags)
            {
                if (contOpt.ContextOptionFlag != ContextOption.SimpleBind && contOpt.Value)
                {
                    contextOption |= (ContextOptions)Enum.Parse(typeof(ContextOptions), contOpt.ContextOptionFlag.ToString());
                }
                else if (contOpt.ContextOptionFlag == ContextOption.SimpleBind && contOpt.Value)
                {
                    sbNeeded = true;
                }
            }
            if (sbNeeded == false)
            {
                contextOption ^= (ContextOptions)Enum.Parse(typeof(ContextOptions), "SimpleBind");
            }
            return contextOption;
        }
    }

    public class Context
    {
        /// <summary>
        /// Auth type
        /// </summary>
        public ContextOption ContextOptionFlag { get; set; }

        /// <summary>
        /// Flag value
        /// </summary>
        public bool Value { get; set; }
    }

    public enum ContextOption
    {
        //
        // Summary:
        //     The client is authenticated by using either Kerberos or NTLM. When the user name
        //     and password are not provided, the Account Management API binds to the object
        //     by using the security context of the calling thread, which is either the security
        //     context of the user account under which the application is running or of the
        //     client user account that the calling thread represents.
        Negotiate = 1,
        //
        // Summary:
        //     The client is authenticated by using the Basic authentication.
        SimpleBind = 2,
        //
        // Summary:
        //     The channel is encrypted by using the Secure Sockets Layer (SSL). Active Directory
        //     requires that the Certificate Services be installed to support SSL.
        SecureSocketLayer = 4,
        //
        // Summary:
        //     The integrity of the data is verified. This flag can only be used with the Negotiate
        //     context option and is not available with the simple bind option.
        Signing = 8,
        //
        // Summary:
        //     The data is encrypted by using Kerberos.
        Sealing = 16,
        //
        // Summary:
        //     Specify this flag when you use the domain context type if the application is
        //     binding to a specific server name.
        ServerBind = 32
    }
}
