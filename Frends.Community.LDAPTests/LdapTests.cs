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
                NewPassword = "",
                SetPassword = false
            };
            var result = LdapActiveDirectoryOperations.AD_CreateUser(_connection, user, e);
            Assert.AreEqual(result.OperationSuccessful, true);
        }


        [Test, Order(2)]
        public void ShouldFetchUser()
        {
            var e = new AD_FetchObjectProperties()
            {
                Filter = "(&(objectClass=user)(cn=" + _user + "))",
                Path = _path
            };
            var u = LdapActiveDirectoryOperations.AD_FetchObjects(_connection, e);
            var result = u[0].GetProperty("cn");
            Assert.AreEqual(result[0], _user);
        }


        [Test, Order(3)]
        public void ShouldGetPropertyLargeInteger()
        {
            var user = new CreateADuser
            {
                CN = _user,
                Path = _path
            };

            var e = new AD_FetchObjectProperties()
            {
                Filter = "(&(objectClass=user)(cn=" + _user + "))",
                Path = _path
            };

            //Assume the created test user has a default value of: accountExpires = 0x7FFFFFFFFFFFFFFF = 9223372036854775807
            System.Int64 largeInt = 9223372036854775807;

            var u = LdapActiveDirectoryOperations.AD_FetchObjects(_connection, e); //user
            var result = (System.Int64)u[0].GetPropertyLargeInteger("accountExpires");
            Assert.AreEqual(largeInt, result);
        }


        [Test, Order(4)]
        public void GetPropertyLargeInteger_InvalidAttributeShouldThrowException()
        {
            var user = new CreateADuser
            {
                CN = _user,
                Path = _path
            };

            var e = new AD_FetchObjectProperties()
            {
                Filter = "(&(objectClass=user)(cn=" + _user + "))",
                Path = _path
            };

            var u = LdapActiveDirectoryOperations.AD_FetchObjects(_connection, e); //user
            Assert.Throws<System.ArgumentException>(() => u[0].GetPropertyLargeInteger("fooBar"));
        }


        [Test, Order(5)]
        public void ShouldGetUserAccountExpiresDateTime()
        {
            var user = new CreateADuser
            {
                CN = _user,
                Path = _path
            };

            var e = new AD_FetchObjectProperties()
            {
                Filter = "(&(objectClass=user)(cn=" + _user + "))",
                Path = _path
            };

            //User accountExpires = 0x7FFFFFFFFFFFFFFF -> DateTime should return DateTime.MaxValue
            System.DateTime expectedDateTime = System.DateTime.MaxValue;

            var u = LdapActiveDirectoryOperations.AD_FetchObjects(_connection, e); //user
            System.DateTime result = u[0].GetAccountExpiresDateTime();
            Assert.AreEqual(expectedDateTime, result);
        }



        [Test, Order(6)]
        public void ShouldUpdateUser()
        {
            var user = new UpdateADuser { DN = _dn };
            var attributes = new List<EntryAttribute> {
                new EntryAttribute {Attribute = AdUserAttribute.description, Value = "MattiMeikalainen", DataType = AttributeType.String}
            };
            user.OtherAttributes = attributes.ToArray();
            var result = LdapActiveDirectoryOperations.AD_UpdateUser(_connection, user);
            Assert.AreEqual(result.OperationSuccessful, true);
        }

        [Test, Order(7)]
        public void ShouldAddGroups()
        {
            var u = new AD_AddGroupsUserProperties { Dn = _dn };
            var e = new AD_AddGroupsProperties { Groups = new[] { _groupDn } };

            var result = LdapActiveDirectoryOperations.AD_AddGroups(_connection, u, e);
            Assert.AreEqual(result.OperationSuccessful, true);
        }

        [Test, Order(8)]
        public void ShouldRemoveUserFromGroup()
        {
            var u = new AD_RemoveFromGroupsTargetProperties { Dn = _dn };
            var e = new AD_RemoveFromGroupsGroupProperties { Groups = new[] { _groupDn } };

            Output result = LdapActiveDirectoryOperations.AD_RemoveFromGroups(_connection, u, e);

            Assert.IsTrue(result.OperationSuccessful);
        }

        [Test, Order(9)]
        public void ShouldDeleteUser()
        {
            var e = new AD_DeleteUserProperties { Cn = _user, Path = _path };
            var result = LdapActiveDirectoryOperations.AD_DeleteUser(_connection, e);
            Assert.AreEqual(result.OperationSuccessful, true);
        }

        /// <summary>
        /// Test for AD_SearchOjects: fetch a property and a not loaded property.
        /// </summary>
        [Test, Order(10)]
        public void ShouldSearchUser()
        {
            var prop = new AD_SearchObjectProperties()
            {
                Filter = "(&(objectClass=user)(cn=" + _user + "))",
                Path = _path,
                PropertiesToLoad = new string[] { "cn", "invalidProperty" }
            };
            
            var ret = LdapActiveDirectoryOperations.AD_SearchObjects(_connection, prop);

            string cnValue = ret[0].GetPropertyStringValue("cn");
            string nullValue = ret[0].GetPropertyStringValue("name"); // should return null
            Assert.AreEqual(cnValue, _user);
            Assert.AreEqual(nullValue, null);
        }
    }
}
