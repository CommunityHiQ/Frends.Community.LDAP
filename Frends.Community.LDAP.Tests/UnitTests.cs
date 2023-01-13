using NUnit.Framework;
using System.Collections.Generic;
using Frends.Community.LDAP.Models;
using System;

namespace Frends.Community.LDAP.Tests
{
    [TestFixture]
    public class LdapTests
    {
        private string _user;
        private string _dn;
        private string _path;
        private string _groupDn;
        private LdapConnectionInfo _connection;
        private PasswordParameters _passConnection;


        [SetUp]
        public void Setup()
        {
            _user = "MattiMeikalainen";
            _path = "CN=Users,DC=FRENDSTest01,DC=net";
            _dn = "CN=MattiMeikalainen;CN=Users,DC=FRENDSTest01,DC=net";
            _groupDn = "CN=Guests,CN=Builtin";

            _connection = new LdapConnectionInfo
            {
                AuthenticationFlags = new [] { new AuthType { AuthenticationFlag = Authentication.Secure, Value = true }  },
                LdapUri = "LDAP://"  + Environment.GetEnvironmentVariable("HiQAzureADTestAddress", EnvironmentVariableTarget.User),
                Username = Environment.GetEnvironmentVariable("HiQAzureADTestUser", EnvironmentVariableTarget.User),
                Password = Environment.GetEnvironmentVariable("HiQAzureADTestPassword", EnvironmentVariableTarget.User),
            };

            _passConnection = new PasswordParameters
            {
                AdServer = Environment.GetEnvironmentVariable("HiQAzureADTestAddress", EnvironmentVariableTarget.User),
                AdContainer = "DC=FRENDSTest01",
                Username = Environment.GetEnvironmentVariable("HiQAzureADTestUser", EnvironmentVariableTarget.User),
                Password = Environment.GetEnvironmentVariable("HiQAzureADTestPassword", EnvironmentVariableTarget.User),

                // UserPrincipalName not confirmed to exist in test AD - TO DO!
                UserPrincipalName = "Matti.Meikalainen@testi.fi",

                // HiQAzureADTestUserNewPassword has not yet been set - TO DO!
                NewPassword = Environment.GetEnvironmentVariable("HiQAzureADTestUserNewPassword", EnvironmentVariableTarget.User),
                ContextOptionFlags = new []
                    {
                    new Context { ContextOptionFlag = ContextOption.SimpleBind, Value = true },
                    new Context { ContextOptionFlag = ContextOption.Signing, Value = true },
                    new Context { ContextOptionFlag = ContextOption.ServerBind , Value = true }
                    }
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
            {   
                new ADFlag {FlagType = ADFlagType.ADS_UF_ACCOUNTDISABLE, Value = false},
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

            var e = new AD_FetchObjectProperties()
            {
                Filter = "(&(objectClass=user)(cn=" + _user + "))",
                Path = _path
            };

            // Assume the created test user has a default value of: accountExpires = 0x7FFFFFFFFFFFFFFF = 9223372036854775807.
            const long largeInt = 9223372036854775807;

            // User.
            var u = LdapActiveDirectoryOperations.AD_FetchObjects(_connection, e);
            var result = (long)u[0].GetPropertyLargeInteger("accountExpires");
            Assert.AreEqual(largeInt, result);
        }


        [Test, Order(4)]
        public void GetPropertyLargeInteger_InvalidAttributeShouldThrowException()
        {

            var e = new AD_FetchObjectProperties()
            {
                Filter = "(&(objectClass=user)(cn=" + _user + "))",
                Path = _path
            };

            // User.
            var u = LdapActiveDirectoryOperations.AD_FetchObjects(_connection, e);
            Assert.Throws<ArgumentException>(() => u[0].GetPropertyLargeInteger("fooBar"));
        }


        [Test, Order(5)]
        public void ShouldGetUserAccountExpiresDateTime()
        {

            var e = new AD_FetchObjectProperties()
            {
                Filter = "(&(objectClass=user)(cn=" + _user + "))",
                Path = _path
            };

            // User accountExpires = 0x7FFFFFFFFFFFFFFF -> DateTime should return DateTime.MaxValue.
            var expectedDateTime = DateTime.MaxValue;

            // User.
            var u = LdapActiveDirectoryOperations.AD_FetchObjects(_connection, e);
            var result = u[0].GetAccountExpiresDateTime();
            Assert.AreEqual(expectedDateTime, result);
        }

        [Test, Order(6)]
        public void ShouldGetUserLastLogonDateTime()
        {
            var e = new AD_FetchObjectProperties()
            {
                Filter = "(&(objectClass=user)(cn=" + _user + "))",
                Path = _path
            };

            var expectedDateTime = DateTime.MinValue;

            var u = LdapActiveDirectoryOperations.AD_FetchObjects(_connection, e);
            var result = u[0].GetPropertyDateTime("lastLogon");
            Assert.AreEqual(expectedDateTime.GetType(), result.GetType());
        }

        [Test, Order(7)]
        public void ShouldUpdateUser()
        {
            var user = new UpdateADuser { DN = _dn };
            var attributes = new List<EntryAttribute>
            {
                new EntryAttribute {Attribute = AdUserAttribute.description, Value = "MattiMeikalainen", DataType = AttributeType.String}
            };
            user.OtherAttributes = attributes.ToArray();
            var result = LdapActiveDirectoryOperations.AD_UpdateUser(_connection, user);
            Assert.AreEqual(result.OperationSuccessful, true);
        }

        [Test, Order(8)]
        public void ShouldAddGroups()
        {
            var u = new AD_AddGroupsUserProperties { Dn = _dn };
            var e = new AD_AddGroupsProperties { Groups = new[] { _groupDn } };

            var result = LdapActiveDirectoryOperations.AD_AddGroups(_connection, u, e);
            Assert.AreEqual(result.OperationSuccessful, true);
        }

        /// <summary>
        /// Test for AD_SearchOjects: fetch a property and a not loaded property.
        /// </summary>
        [Test, Order(8)]
        public void ShouldSearchUser()
        {
            var prop = new AD_SearchObjectProperties()
            {
                Filter = "(&(objectClass=user)(cn=" + _user + "))",
                Path = _path,
                PropertiesToLoad = new[] { "cn", "invalidProperty" }
            };

            var ret = LdapActiveDirectoryOperations.AD_SearchObjects(_connection, prop);

            var cnValue = ret[0].GetPropertyStringValue("cn");

            // Should return null.
            var nullValue = ret[0].GetPropertyStringValue("name");
            Assert.AreEqual(cnValue, _user);
            Assert.AreEqual(nullValue, null);
        }

        /// <summary>
        /// Test for AD_SearchOjects: fetch a property and a not loaded property.
        /// </summary>
        [Test, Order(8)]
        public void ShouldSearchMultipleUsers()
        {
            var prop = new AD_SearchObjectProperties()
            {
                Filter = "(&(objectClass=user)(cn=" + _user + "))",
                Path = _path,
                PropertiesToLoad = Array.Empty<string>()
            };

            var ret = LdapActiveDirectoryOperations.AD_SearchObjects(_connection, prop);

            var cnValue = ret[0].GetPropertyCollectionValues("cn");

            // Should return null.
            var objectclass = ret[0].GetPropertyCollectionValues("objectclass");
            Assert.AreEqual(new List<string> { _user }, cnValue);
            Assert.AreEqual(new List<string> { "top", "person", "organizationalPerson", "user" }, objectclass);
        }

        [Test, Order(9)]
        public void ShouldRemoveUserFromGroup()
        {
            var u = new AD_RemoveFromGroupsTargetProperties { Dn = _dn };
            var e = new AD_RemoveFromGroupsGroupProperties { Groups = new[] { _groupDn } };

            var result = LdapActiveDirectoryOperations.AD_RemoveFromGroups(_connection, u, e);

            Assert.IsTrue(result.OperationSuccessful);
        }

        [Test, Order(10)]
        public void ShouldDeleteUser()
        {
            var e = new AD_DeleteUserProperties { Cn = _user, Path = _path };
            var result = LdapActiveDirectoryOperations.AD_DeleteUser(_connection, e);
            Assert.AreEqual(result.OperationSuccessful, true);
        }



        /// <summary>
        /// Test for AD_SearchOjects: fetch a property and a not loaded property.
        /// </summary>
        [Test, Order(12)]
        [Ignore("Test is not implemented. TODO")]
        public void ShouldMoveUser()
        {
            var input = new MoveObject()
            {
                CN = "kk",
                Path = "kk",
                NewPath = "kk"
            };

            var ret = LdapActiveDirectoryOperations.AD_MoveObject(_connection, input);

            // Also assert that copy of object is correct.
            Assert.AreEqual(ret.OperationSuccessful, true);
        }

        /// <summary>
        ///  Test for AD_SetUserPassword: Set a user's password.
        /// </summary>
        [Test, Order(12)]
        [Ignore("Test is not working on build server. TODO")]

        public void ShouldSetPassword()
        {
            var result = LdapActiveDirectoryOperations.AD_SetUserPassword(_passConnection);
            Assert.AreEqual(result.OperationSuccessful, true);
        }
    }
}
