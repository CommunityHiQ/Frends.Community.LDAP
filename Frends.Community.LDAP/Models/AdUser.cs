using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


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

        public CreateADuser()
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

        public UpdateADuser()
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
