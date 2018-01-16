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
        public void CreateWindowsUser()
        {
            var connection = new LdapConnectionInfo()
            {
                LdapUri = "WinNT://" + Environment.MachineName + ",computer",
                Username = "Administrator",
                Password = "k00k05m41t0!",
                AuthenticationType = Authentication.None
            };
            var user = new WindowsUser();
            user.UserName = "Frendsssssssssssssss";
            user.Password = "Passu123123";
            user.Description = "User created by Frends.";
            LdapOperations.WindowsUser_Create(connection, user);
        }

        [Test]
        public void UpdateWindowsUser()
        {
            var connection = new LdapConnectionInfo()
            {
                LdapUri = "WinNT://" + Environment.MachineName + ",computer",
                Username = "Administrator",
                Password = "k00k05m41t0!",
                AuthenticationType = Authentication.None
            };
            var user = new WindowsUser();
            user.UserName = "Frends";
            user.Password = "Passu1231231";
            user.Description = "User created by Frends.222";
            LdapOperations.WindowsUser_Update(connection, user);
        }

        [Test]
        public void AddWindowsUserToGroup()
        {
            var connection = new LdapConnectionInfo()
            {
                LdapUri = "WinNT://" + Environment.MachineName + ",computer",
                Username = "Administrator",
                Password = "k00k05m41t0!",
                AuthenticationType = Authentication.None
            };

            var user = new WindowsUser();
            user.UserName = "Frends";
            var groups = new List<string>();
            groups.Add("Administrators");
            LdapOperations.WindowsUser_AddToGroup(connection, user, groups.ToArray());
        }

        [Test]
        public void AD_VerifyUserExists()
        {
            var connection = new LdapConnectionInfo()
            {
                AuthenticationType = Authentication.Secure,
                LdapUri = "LDAP://yourInstance",
                Username = "username",
                Password = "password"
            };

            var u = LdapOperations.AD_UserExists(connection, "cn", "MattiMeikalainen2");

            Assert.AreEqual(true, u);
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
            user.OU = "OU=Users,DC=JessenInstanssi";
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

            LdapOperations.AD_CreateUser(connection, user, false ,"");
        }

        [Test]
        public void AD_Update()
        {
            var connection = new LdapConnectionInfo()
            {
                AuthenticationType = Authentication.None,
                LdapUri = "LDAP://127.0.0.1:389",
                Username = "DEVDOM\\Administrator",
                Password = "k00k05m41t0!"
            };

            var user = new AdUser();
            user.CN = "MattiMeikalainen";
            user.OU = "OU=FrendsOU,DC=DEVDOM,DC=COM";
            var attributes = new List<EntryAttribute>();
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.samAccountName, Value = "Matti", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.userPassword, Value = "vaihdettu", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.givenName, Value = "vaihdettu", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.sn, Value = "vaihdettu", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.streetAddress, Value = "vaihdettu", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.telephoneNumber, Value = "vaihdettu", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.l, Value = "vaihdettu", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.postalCode, Value = "vaihdettu", DataType = AttributeType.String });
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.mobile, Value = "vaihdettu", DataType = AttributeType.String });
            user.OtherAttributes = attributes.ToArray();
            LdapOperations.AD_UpdateUser(connection, user);
        }
    }
}
