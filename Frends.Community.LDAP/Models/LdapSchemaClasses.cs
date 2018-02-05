using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS1591 

namespace Frends.Community.LDAP.Models
{
    public static class LdapClasses
    {
        public const string User = "user";
        public const string Group = "group";
        public const string Person = "person";
    }

    public static class LdapMethods
    {
        public const string IsMember = "IsMember";
        public const string Add = "Add";
        public const string Put = "Put";

    }

}
