#pragma warning disable CS1591 

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
