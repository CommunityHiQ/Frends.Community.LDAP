using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using Frends.Community.LDAP.Models;
using Frends.Community.LDAP;

namespace Frends.Community.LDAPTests
{

    [TestFixture]
    public class LdapTests
    {
        [Test]
        public void AD_VerifyUserExists()
        {
            var connection = new LdapConnectionInfo()
            {
                AuthenticationType = Authentication.Secure,
                LdapUri = "",
                Username = "",
                Password = ""
            };

            var e = new AD_FetchObjectProperties()
            {
                filter = @"(&(objectClass=user)(sAMAccountName=Guest))"
            };
            List<OutputObjectEntry> u = LdapActiveDirectoryOperations.AD_FetchObjects(connection, e);
            var result = u[0].GetProperty("sAMAccountName");//lastLogon; dSCorePropagationData; objectClass; whenChanged; GetPropertyLargeInteger
            Assert.AreEqual(4, 1);
        }

        [Test]
        public void AD_Create()
        {
            var connection = new LdapConnectionInfo()
            {
                AuthenticationType = Authentication.Secure,
                LdapUri = "LDAP://yourInstanceHere",
                Username = "username",
                Password = "password"
            };
        
            var user = new AdUser();
            user.CN = "MattiMeikalainen2";
            user.OU = "OU=Users,DC=JessenInstanssi";// OU=Users,DC=JessenInstanssi
            var attributes = new List<EntryAttribute>();
            //attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.samAccountName, Value = "Matti2", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.userPassword, Value = "$ala San4?", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.givenName, Value = "Matti", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.sn, Value = "Meikäläinen", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.streetAddress, Value = "Kuusitie 12", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.telephoneNumber, Value = "040121915", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.l, Value = "Finland", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.postalCode, Value = "00100", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.mobile, Value = "040 25153143", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.initials, Value = "M.M", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.mail, Value = "M.M", DataType = AttributeType.String });
            user.OtherAttributes = attributes.ToArray();

            //var flags = new List<ADFlag>();
            //flags.Add(new ADFlag() { FlagType = ADFlagType.ADS_UF_ACCOUNTDISABLE, Value = false });
            //flags.Add(new ADFlag() { FlagType = ADFlagType.ADS_UF_NORMAL_ACCOUNT, Value = true });

            //user.ADFlags = flags.ToArray();

            //LdapOperations.AD_CreateUser(connection, user, false ,"");
        }

        [Test]
        public void AD_Update()
        {
            var connection = new LdapConnectionInfo()
            {
                AuthenticationType = Authentication.None,
                LdapUri = "",
                Username = "",
                Password = ""
            };

            var user = new AdUser();
            user.CN = "Guest";
            user.OU = "CN=Users,DC=FRENDSTest01,DC=net";//CN=Users,DC=FRENDSTest01,DC=net OU=FrendsOU,DC=DEVDOM,DC=COM"
            var attributes = new List<EntryAttribute>();
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.samAccountName, Value = "Guest2", DataType = AttributeType.String });
            //attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.userPassword, Value = "vaihdettu", DataType = AttributeType.String });
            //attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.givenName, Value = "vaihdettu", DataType = AttributeType.String });
            //attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.sn, Value = "vaihdettu", DataType = AttributeType.String });
            //attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.streetAddress, Value = "vaihdettu", DataType = AttributeType.String });
            //attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.telephoneNumber, Value = "vaihdettu", DataType = AttributeType.String });
            //attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.l, Value = "vaihdettu", DataType = AttributeType.String });
            //attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.postalCode, Value = "vaihdettu", DataType = AttributeType.String });
            //attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.mobile, Value = "vaihdettu", DataType = AttributeType.String });
            user.OtherAttributes = attributes.ToArray();
            LdapActiveDirectoryOperations.AD_UpdateUser(connection, user);
        }

        [Test]
        public void Test()
        {
            AD_VerifyUserExists();
            AD_Update();
        }
    }
}
