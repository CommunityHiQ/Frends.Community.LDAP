using NUnit.Framework;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Frends.Community.LDAP.Models;
using Frends.Community.LDAP;
using TestConfigurationHandler;

namespace Frends.Community.LDAPTests
{
    [TestFixture]
    public class LdapTests
    {
        private string _user;
        private string _dn;
        private string _path;
        private string _groupDn;
        private LdapConnectionInfo _connection;


        [SetUp]
        public void Setup()
        {
            _user = "MattiMeikalainen";
            _path = "CN=Users,DC=FRENDSTest01,DC=net";
            _dn = "CN=MattiMeikalainen;CN=Users,DC=FRENDSTest01,DC=net";
            _groupDn = "CN=Guests,CN=Builtin";

            _connection = new LdapConnectionInfo
            {
                AuthenticationType = Authentication.Secure,
                LdapUri = "LDAP://"  + ConfigHandler.ReadConfigValue("HiQ.AzureADTest.Address"),
                Username = ConfigHandler.ReadConfigValue("HiQ.AzureADTest.User"),
                Password = ConfigHandler.ReadConfigValue("HiQ.AzureADTest.Password")
            };
        }

        [Test, Order(1)]
        public void ShouldCreateUser()
        {
            var user = new CreateADuser
            {
                CN = _user,
                Path = _path
            };
            var attributes = new List<EntryAttribute> { new EntryAttribute() { Attribute = AdUserAttribute.givenName, Value = "Matti", DataType = AttributeType.String } };
            user.OtherAttributes = attributes.ToArray();

            var flags = new List<ADFlag>
            { new ADFlag {FlagType = ADFlagType.ADS_UF_ACCOUNTDISABLE, Value = false},
                new ADFlag {FlagType = ADFlagType.ADS_UF_NORMAL_ACCOUNT, Value = true}
            };
            user.ADFlags = flags.ToArray();

            var e = new AD_CreateUserProperties()
            {
                newPassword = "",
                setPassword = false
            };
            var result = LdapActiveDirectoryOperations.AD_CreateUser(_connection, user, e);
            Assert.AreEqual(result.operationSuccessful, true);
        }


        [Test, Order(2)]
        public void ShouldFetchUser()
        {
            var e = new AD_FetchObjectProperties()
            {
                filter = "(&(objectClass=user)(cn=" + _user + "))",
                Path = _path
            };
            var u = LdapActiveDirectoryOperations.AD_FetchObjects(_connection, e);
            var result = u[0].GetProperty("cn");
            Assert.AreEqual(result[0], _user);
        }


        [Test, Order(3)]
        public void ShouldUpdateUser()
        {
            var user = new UpdateADuser { DN = _dn };
            var attributes = new List<EntryAttribute> {
                new EntryAttribute {Attribute = AdUserAttribute.description, Value = "MattiMeikalainen", DataType = AttributeType.String}
            };
            user.OtherAttributes = attributes.ToArray();
            var result = LdapActiveDirectoryOperations.AD_UpdateUser(_connection, user);
            Assert.AreEqual(result.operationSuccessful, true);
        }

        [Test, Order(4)]
        public void ShouldAddGroups()
        {
            var u = new AD_AddGroupsUserProperties { dn = _dn };
            var e = new AD_AddGroupsProperties { groups = new[] { _groupDn } };

            var result = LdapActiveDirectoryOperations.AD_AddGroups(_connection, u, e);
            Assert.AreEqual(result.operationSuccessful, true);
        }

        [Test, Order(5)]
        public void ShouldRemoveUserFromGroup()
        {
            var u = new AD_RemoveFromGroupsTargetProperties { Dn = _dn };
            var e = new AD_RemoveFromGroupsGroupProperties { Groups = new[] { _groupDn } };

            Output result = LdapActiveDirectoryOperations.AD_RemoveFromGroups(_connection, u, e);

            Assert.IsTrue(result.operationSuccessful);
        }

        [Test, Order(6)]
        public void ShouldDeleteUser()
        {
            var e = new AD_DeleteUserProperties { Cn = _user, Path = _path };
            var result = LdapActiveDirectoryOperations.AD_DeleteUser(_connection, e);
            Assert.AreEqual(result.operationSuccessful, true);
        }
    }
}
