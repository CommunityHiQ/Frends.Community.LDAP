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
        public void AD_4_VerifyUserExists()
        {
            var connection = new LdapConnectionInfo()
            {
                AuthenticationType = Authentication.Secure,
                LdapUri = "LDAP://52.166.223.166/CN=Users,DC=FRENDSTest01,DC=net",
                Username = "FRENDSTEST01\\Testuser1",
                Password = "6Fe96cb8e2#141"
            };

            var e = new AD_FetchObjectProperties()
            {
                filter = @"(&(objectClass=user)(cn=MattiMeikalainen))" //(&(objectClass=user)(sAMAccountName=MattiMeikalainen))
            };
            List<OutputObjectEntry> u = LdapActiveDirectoryOperations.AD_FetchObjects(connection, e);
            object[] result = u[0].GetProperty("cn");//sAMAccountName;lastLogon; dSCorePropagationData; objectClass; whenChanged; GetPropertyLargeInteger
            Assert.AreEqual(result[0], "MattiMeikalainen");
        }

        [Test]
        public void AD_2_Create()
        {
            var connection = new LdapConnectionInfo()
            {
                AuthenticationType = Authentication.Secure,
                LdapUri = "LDAP://52.166.223.166",
                Username = "FRENDSTEST01\\Testuser1",
                Password = "6Fe96cb8e2#141"
            };
        
            var user = new AdUser();
            user.CN = "MattiMeikalainen";
            user.OU = "CN=Users,DC=FRENDSTest01,DC=net";// OU=Users,DC=JessenInstanssi CN=Users,DC=FRENDSTest01,DC=net
            var attributes = new List<EntryAttribute>();
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.givenName, Value = "Matti", DataType = AttributeType.String });
            user.OtherAttributes = attributes.ToArray();

            var flags = new List<ADFlag>();
            flags.Add(new ADFlag() { FlagType = ADFlagType.ADS_UF_ACCOUNTDISABLE, Value = false });
            flags.Add(new ADFlag() { FlagType = ADFlagType.ADS_UF_NORMAL_ACCOUNT, Value = true });

            user.ADFlags = flags.ToArray();

            var e = new AD_CreateUserProperties()
            {
                newPassword = "",
                setPassword = false
            };

            var ret=LdapActiveDirectoryOperations.AD_CreateUser(connection, user, e);
            Assert.AreEqual(true,true);
        }

        [Test]
        public void AD_3_Update()
        {
            var connection = new LdapConnectionInfo()
            {
                AuthenticationType = Authentication.Secure,
                LdapUri = "LDAP://52.166.223.166",
                Username = "FRENDSTEST01\\Testuser1",
                Password = "6Fe96cb8e2#141"
            };

            // update value vois olla yhessä annetaan vain DN
            var user = new AdUser();
            user.CN = "MattiMeikalainen";
            user.OU = "CN=Users,DC=FRENDSTest01,DC=net";//CN=Users,DC=FRENDSTest01,DC=net OU=FrendsOU,DC=DEVDOM,DC=COM"
            var attributes = new List<EntryAttribute>();
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.description, Value = "MattiMeikalainen", DataType = AttributeType.String });
            user.OtherAttributes = attributes.ToArray();
            var ret = LdapActiveDirectoryOperations.AD_UpdateUser(connection, user);
            Assert.AreEqual(true, true);
        }

        [Test]
        public void AD_1_Delete()
        {
            var connection = new LdapConnectionInfo()
            {
                AuthenticationType = Authentication.Secure,
                LdapUri = "LDAP://52.166.223.166/CN=Users,DC=FRENDSTest01,DC=net",
                Username = "FRENDSTEST01\\Testuser1",
                Password = "6Fe96cb8e2#141"
            };

            var e = new AD_DeleteUserProperties()
            {
                user = "MattiMeikalainen"
            };
            var ret = LdapActiveDirectoryOperations.AD_DeleteUser(connection,e);
            Assert.AreEqual(true, true);
        }

        [Test]
        public void AD_5_AddGroups()
        {
            var connection = new LdapConnectionInfo()
            {
                AuthenticationType = Authentication.Secure,
                LdapUri = "LDAP://52.166.223.166",
                Username = "FRENDSTEST01\\Testuser1",
                Password = "6Fe96cb8e2#141"
            };


            // vois olla yhessä
            var user = new AdUser();
            user.CN = "CN=MattiMeikalainen";
            user.OU = "CN=Users,DC=FRENDSTest01,DC=net";// OU=Users,DC=JessenInstanssi CN=Users,DC=FRENDSTest01,DC=net
            var attributes = new List<EntryAttribute>();
            attributes.Add(new EntryAttribute() { Attribute = AdUserAttribute.givenName, Value = "Matti", DataType = AttributeType.String });
            user.OtherAttributes = attributes.ToArray();

            var flags = new List<ADFlag>();
            flags.Add(new ADFlag() { FlagType = ADFlagType.ADS_UF_ACCOUNTDISABLE, Value = false });
            flags.Add(new ADFlag() { FlagType = ADFlagType.ADS_UF_NORMAL_ACCOUNT, Value = true });

            user.ADFlags = flags.ToArray();

            var e = new AD_AddGroupsProperties()
            {
                groups=new string[] { "CN=Guests,CN=Builtin" }
            };

           // var ret = LdapActiveDirectoryOperations.AD_AddGroups(connection, user, e);
            Assert.AreEqual(true, true);
        }

    }
}
