using Frends.Community.LDAP.Models;
using System.DirectoryServices;


#pragma warning disable CS1591 

namespace Frends.Community.LDAP.Services
{
    public static class DirectoryEntryExtensions
    {

        public static void SetAccountFlag(this DirectoryEntry entry, ADFlagType flag, bool value)
        {
            if (entry.Properties["userAccountControl"].Value == null)
                entry.Properties["userAccountControl"].Value = 0;
            var currentValue = (int) entry.Properties["userAccountControl"].Value;
            var newFlag = (int)flag;

            if (value)
                entry.Properties["userAccountControl"].Value = currentValue | newFlag;
            else
                entry.Properties["userAccountControl"].Value = currentValue & ~newFlag;


        }

    }
}
