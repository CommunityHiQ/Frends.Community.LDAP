using System.ComponentModel;

#pragma warning disable CS1591 

namespace Frends.Community.LDAP.Models
{
    public class WindowsUser : LdapEntry
    {
        [DefaultValue("")]
        [Description("User name for windows user.")]
        public string UserName { get; set; }

        [PasswordPropertyText(true)]
        [DefaultValue("")]
        [Description("Password for user")]       
        public string Password { get; set; }

        [DefaultValue(false)]
        [Description("Set password?")]
        public bool SetPassword { get; set; }

        [DefaultValue("")]
        [Description("Description for user")]
        public string Description { get; set; }

        public override EntryAttribute[] OtherAttributes { get; set; }

    }
}
