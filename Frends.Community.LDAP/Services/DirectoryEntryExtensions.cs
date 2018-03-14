
using Frends.Community.LDAP.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS1591 

namespace Frends.Community.LDAP.Services
{
    public static class DirectoryEntryExtensions
    {

        public static void SetAccountFlag(this DirectoryEntry entry, ADFlagType flag, bool value)
        {
            if (entry.Properties["userAccountControl"].Value == null)
                entry.Properties["userAccountControl"].Value = 0;
            int currentValue = (int) entry.Properties["userAccountControl"].Value;
            int newFlag = (int)flag;

            if (value)
                entry.Properties["userAccountControl"].Value = currentValue | newFlag;
            else
                entry.Properties["userAccountControl"].Value = currentValue & ~newFlag;


        }

    }
}
