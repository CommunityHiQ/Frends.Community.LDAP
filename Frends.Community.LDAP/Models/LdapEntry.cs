using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.Community.LDAP.Models
{
    public abstract class LdapEntry
    {
        public LdapEntry()
        {
            OtherAttributes = new EntryAttribute[0];
        }

        public abstract EntryAttribute[] OtherAttributes { get; set; }
    }
}
