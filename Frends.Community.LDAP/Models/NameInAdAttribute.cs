using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
