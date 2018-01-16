using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frends.Community.LDAP.Models
{
    public class AdUser : LdapEntry
    {

        public AdUser() : base()
        {
            ADFlags = new ADFlag[0];
        }

        /// <summary>
        /// Common name, i.e. JohnDoe. Mandatory.
        /// </summary>
        public string CN { get; set; }


        /// <summary>
        /// Organization Unit, where the user is located
        /// </summary>
        public string OU { get; set; }

        public ADFlag[] ADFlags {get;set;}

        public override EntryAttribute[] OtherAttributes{ get; set; }

        public string GetPath()
        {
            return CN + "," + OU;
        }
    }
}
