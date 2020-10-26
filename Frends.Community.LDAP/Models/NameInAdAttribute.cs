using System;

#pragma warning disable CS1591 

namespace Frends.Community.LDAP.Models
{
    public class LdapEntryAttribute : Attribute
    {
        public string Name { get; set; }
        public LdapEntryAttribute(string name)
        {
            Name = name;
        }
    }
}
