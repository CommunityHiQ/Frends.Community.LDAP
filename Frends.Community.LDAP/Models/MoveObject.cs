using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices;


#pragma warning disable CS1591 

namespace Frends.Community.LDAP.Models
{
    public class MoveObject
    {
        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("CN=MattiMeikalainen")]
        public string CN { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("CN=Users,DC=FRENDSTest01,DC=net")]
        public string Path { get; set; }

        [DisplayFormat(DataFormatString = "Text")]
        [DefaultValue("CN=Users,DC=FRENDSTest01,DC=net")]
        public string NewPath { get; set; }
    }
}
