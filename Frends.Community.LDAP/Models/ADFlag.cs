using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS1591 

namespace Frends.Community.LDAP.Models
{
    public enum ADFlagType
    {
        ADS_UF_SCRIPT = 0x1,
        ADS_UF_ACCOUNTDISABLE = 0x2,
        ADS_UF_HOMEDIR_REQUIRED = 0x8,
        ADS_UF_LOCKOUT = 0x10,
        ADS_UF_PASSWD_NOTREQD = 0x20,
        ADS_UF_PASSWD_CANT_CHANGE = 0x40,
        ADS_UF_ENCRYPTED_TEXT_PASSWORD_ALLOWED = 0x80,
        ADS_UF_TEMP_DUPLICATE_ACCOUNT = 0x100,
        ADS_UF_NORMAL_ACCOUNT = 0x200,
        ADS_UF_INTERDOMAIN_TRUST_ACCOUNT = 0x800,
        ADS_UF_WORKSTATION_TRUST_ACCOUNT = 0x1000,
        ADS_UF_SERVER_TRUST_ACCOUNT = 0x2000,
        ADS_UF_DONT_EXPIRE_PASSWD = 0x10000,
        ADS_UF_MNS_LOGON_ACCOUNT = 0x20000,
        ADS_UF_SMARTCARD_REQUIRED = 0x40000,
        ADS_UF_TRUSTED_FOR_DELEGATION = 0x80000,
        ADS_UF_NOT_DELEGATED = 0x100000,
        ADS_UF_USE_DES_KEY_ONLY = 0x200000,
        ADS_UF_DONT_REQUIRE_PREAUTH = 0x400000,
        ADS_UF_PASSWORD_EXPIRED = 0x800000,
        ADS_UF_TRUSTED_TO_AUTHENTICATE_FOR_DELEGATION = 0x1000000,
    }

    public enum AdUserAttribute
    {
        CustomAttribute,
        samAccountName,
        cn,
        userPassword,
        givenName,
        sn,
        initials,
        displayName,
        description,
        physicalDeliveryOfficeName,
        telephoneNumber,
        mail,
        streetAddress,
        postOfficeBox,
        l,
        st,
        postalCode,
        co,
        homePhone,
        mobile,
        facsimileTelephoneNumber,
        title,
        department,
        company
    }
}
