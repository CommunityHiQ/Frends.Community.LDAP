using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS1591 

namespace Frends.Community.LDAP.Models
{

    public class CreateADuser : LdapEntry
    {
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("CN=MattiMeikalainen")]
        public string CN { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("CN=Users,DC=FRENDSTest01,DC=net")]
        public string Path { get; set; }

        public CreateADuser() : base()
        {
            ADFlags = new ADFlag[0];
        }

        public ADFlag[] ADFlags { get; set; }

        public override EntryAttribute[] OtherAttributes { get; set; }

        public string GetPath()
        {
            return "";
            //return CN+","+OU;
        }

    }

    public class UpdateADuser : LdapEntry
    {
        [DefaultValue("CN=MattiMeikalainen,CN=Users,DC=FRENDSTest01,DC=net")]
        [DisplayFormat(DataFormatString = "Text")]
        public string DN { get; set; }

        public UpdateADuser() : base()
        {
            ADFlags = new ADFlag[0];
        }

        public ADFlag[] ADFlags { get; set; }

        public override EntryAttribute[] OtherAttributes { get; set; }

        public string GetPath()
        {
            return "";
            //return CN+","+OU;
        }
    }
}
