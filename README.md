# Frends.Community.LDAP

FRENDS Community Task for Active Directory related operations.

Task operations that use Active Directory library.
https://github.com/CommunityHiQ/Frends.Community.LDAP

- [Installing](#installing)
- [Tasks](#tasks)
     - [AD_FetchObject](#ad_fetchbobject)
     - [AD_CreateUser](#ad_createuser)
     - [AD_UpdateUser](#ad_updateuser)
     - [AD_AddGroups](#ad_addgroups)
     - [AD_RemoveFromGroups](#ad_removefromgroups)
     - [AD_DeleteUser](#ad_deleteuser)
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)

# Installing

You can install the task via FRENDS UI Task View or you can find the NuGet package from the following NuGet feed
https://www.myget.org/F/frends-community/api/v3/index.json

# Tasks

## AD_FetchObject
Searches Active Directory for an object(s) specified by path and filter.

### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| LDAP uri | string | Actice Directory uri | 'LDAP://xx.xxx.xxx.xxx' |
| Username | string | User name | 'user' |
| Password | string | Password | '****' |
| Authentication type | enum: None, Secure, Encryption, SecureSocketsLayer, ReadonlyServer, Anonymous, FastBind, Signing, Sealing, Delegation, ServerBind  | Authentication type | https://msdn.microsoft.com/en-us/library/system.directoryservices.authenticationtypes(v=vs.110).aspx |
| Path | string | The path to be search attribute. | 'CN=Users,DC=FRENDSTest01,DC=net' |
| filter | string | Attribute name to be searched. | '(&(objectClass=user)(sAMAccountName=TestAdmin))' |

### Returns

A result object(s) with parameters.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| OutputObjectEntry | The collection of the DirectoryEntry | Found objects | |

https://msdn.microsoft.com/en-us/library/system.directoryservices.directoryentry(v=vs.110).aspx

Usage:
To fetch result use syntax:

#result[0].GetProperty("instanceType")

#result[0].GetPropertyLargeInteger("lastLogon") - If attribute is in integer8 format.

#result[0].GetAccountExpiresDateTime() - Returns a DateTime value for AD accountExpires attribute. If accountExpires=0x7FFFFFFFFFFFFFFF, DateTime.MaxValue is returned.

#result can be looped by loop element, also.

## AD_CreateUser
Create a user to AD.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| LDAP uri | string | Actice Directory uri | 'LDAP://xx.xxx.xxx.xxx' |
| Username | string | User name | 'user' |
| Password | string | Password | '****' |
| Authentication type | enum: None, Secure, Encryption, SecureSocketsLayer, ReadonlyServer, Anonymous, FastBind, Signing, Sealing, Delegation, ServerBind | Authentication type | https://msdn.microsoft.com/en-us/library/system.directoryservices.authenticationtypes(v=vs.110).aspx|
| Cn | string | Common name | 'CN=MattiMeikalainen' |
| Path | string | Location, where the user is located. | CN=Users,DC=FRENDSTest01,DC=net |
| Ad flags | List | https://msdn.microsoft.com/en-us/library/ms680832(v=vs.85).aspx |  |
| Other attributes | List | parameters: Attribute=attribute name; Value: Value to be set; Data type: Attribute's type |  |

### Returns

An object with a parameter.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| operationSuccessful | bool | True, if operation is successful | |


## AD_UpdateUser
Update a user in the AD.

### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| LDAP uri | string | Actice Directory uri | 'LDAP://xx.xxx.xxx.xxx' |
| Username | string | User name | 'user' |
| Password | string | Password | '****' |
| Authentication type | enum: None, Secure, Encryption, SecureSocketsLayer, ReadonlyServer, Anonymous, FastBind, Signing, Sealing, Delegation, ServerBind | Authentication type | https://msdn.microsoft.com/en-us/library/system.directoryservices.authenticationtypes(v=vs.110).aspx |
| DN | string | distinguishedName, where the user is located. | 'CN=MattiMeikalainen,CN=Users,DC=FRENDSTest01,DC=net' |
| Ad flags | List | https://msdn.microsoft.com/en-us/library/ms680832(v=vs.85).aspx |  |
| Other attributes | List | parameters: Attribute=attribute name; Value: Value to be set(set to null if you want to clear the value); Data type: Attribute's type |  |

### Returns
An object with parameters.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| operationSuccessful | bool | True, if user found| |
| user | DirectoryEntry(object) | Updated user | |

## AD_AddGroups
Add the user in AD to a group(s).

### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| LDAP uri | string | Actice Directory uri | 'LDAP://xx.xxx.xxx.xxx' |
| Username | string | User name | 'user' |
| Password | string | Password | '****' |
| Authentication type | enum: None, Secure, Encryption, SecureSocketsLayer, ReadonlyServer, Anonymous, FastBind, Signing, Sealing, Delegation, ServerBind | Authentication type | https://msdn.microsoft.com/en-us/library/system.directoryservices.authenticationtypes(v=vs.110).aspx |
| DN | string | distinguishedName, where the user is located. | 'CN=MattiMeikalainen,CN=Users,DC=FRENDSTest01,DC=net' |
| AD_AddGroupsProperties| List | Groups the user to be added. | 'CN=Guests,CN=Builtin' |

### Returns: 
Result a object with parameters.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| operationSuccessful | bool | True, if operation is successful | |

## AD_RemoveFromGroups
Removes AD object from a set of groups

### Properties
Ldap connection Info:

| Property           | Type   | Description             | Example                |
| ------------------ | ------ | ----------------------- | ---------------------- |
| LdapUri            | string | Uri for the LDAP server | `LDAP://frends.ad.org` |
| Username           | string | User name to login with | `frendsAgent`          |
| Password           | string | Password                | `***`                  |
| AuthenticationType | Enum   | Type of authentication  | `Secure` (see https://docs.microsoft.com/en-us/dotnet/api/system.directoryservices.authenticationtypes?redirectedfrom=MSDN&view=netframework-4.7.2) |

Target:

| Property           | Type   | Description             | Example                |
| ------------------ | ------ | ----------------------- | ---------------------- |
| Dn                 | string | Distinguished name of the object to remove from groups | `CN=MattiMeikalainen,CN=Users,DC=FRENDSTest01,DC=net` |

Groups to remove from:

| Property           | Type     | Description             | Example                |
| ------------------ | -------- | ----------------------- | ---------------------- |
| Groups             | string[] | List of DN strings identifying the groups from which the target should be removed from | `CN=Guests,CN=Builtin` |

### Returns
An object with parameters.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| operationSuccessful | bool | True, if operation is successful | |

## AD_DeleteUser
Deletes AD user

### Properties
Ldap connection Info:

| Property           | Type   | Description             | Example                |
| ------------------ | ------ | ----------------------- | ---------------------- |
| LdapUri            | string | Uri for the LDAP server | `LDAP://frends.ad.org` |
| Username           | string | User name to login with | `frendsAgent`          |
| Password           | string | Password                | `***`                  |
| AuthenticationType | Enum   | Type of authentication  | `Secure` (see https://docs.microsoft.com/en-us/dotnet/api/system.directoryservices.authenticationtypes?redirectedfrom=MSDN&view=netframework-4.7.2) |

User:

| Property           | Type   | Description             | Example                |
| ------------------ | ------ | ----------------------- | ---------------------- |
| Path               | string | Path to the OU where the user is located | `OU=Users,DC=FRENDSTest01,DC=net` |
| Cn                 | string | CN of the user to be deleted | `UserName` |


### Returns
An object with parameters.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| operationSuccessful | bool | True, if operation is successful | `true` |

## AD_RenameUser
Renames AD user (changes user's CN)

### Properties
Ldap connection Info:

| Property           | Type   | Description             | Example                |
| ------------------ | ------ | ----------------------- | ---------------------- |
| LdapUri            | string | Uri for the LDAP server | `LDAP://frends.ad.org` |
| Username           | string | User name to login with | `frendsAgent`          |
| Password           | string | Password                | `***`                  |
| AuthenticationType | Enum   | Type of authentication  | `Secure` (see https://docs.microsoft.com/en-us/dotnet/api/system.directoryservices.authenticationtypes?redirectedfrom=MSDN&view=netframework-4.7.2) |

User:

| Property           | Type   | Description             | Example                |
| ------------------ | ------ | ----------------------- | ---------------------- |
| Path               | string | Path to the OU where the user is located | `OU=Users,DC=FRENDSTest01,DC=net` |
| Cn                 | string | Current CN of the user | `UserName` |
| NewCn              | string | New CN that is to be assigned to the user | `NewUserName` |


### Returns
An object with parameters.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| operationSuccessful | bool | True, if operation is successful | `true` |

# Building

Clone a copy of the repo

`git clone https://github.com/CommunityHiQ/Frends.Community.LDAP`

Restore dependencies

`nuget restore FreFrends.Community.LDAP`

Rebuild the project

Run Tests with nunit3. Tests can be found under

`Frends.Community.LDAP\bin\Release\Frends.Community.LDAPTests.dll`

Create a NuGet package

`nuget pack Frends.Community.LDAP.nuspec`

# Contributing
When contributing to this repository, please first discuss the change you wish to make via issue, email, or any other method with the owners of this repository before making a change.

1. Fork the repo on GitHub
2. Clone the project to your own machine
3. Commit changes to your own branch
4. Push your work back up to your fork
5. Submit a Pull request so that we can review your changes

NOTE: Be sure to merge the latest from "upstream" before making a pull request!

# Change Log

| Version | Changes |
| ------- | ------- |
| 1.0.0   | First version. Includes AD_FetchObjects, AD_CreateUser, AD_UpdateUser, AD_AddGroups |
| 2.0.0   | Added AD_RemoveFromGroups |
| 2.1.0   | AD flags are now updated by UpdateADuser |
| 3.0.0   | Added AD_DeleteUser |
| 3.1.0   | AD_UpdateUser now handles empty attributes properly | 
<<<<<<< HEAD
| 3.1.2   | Added AD_RenameUser |
| 3.2.0   | Added new attribute type: JSONArray |
=======
| 3.2.0   | Added GetAccountExpiresDateTime, fixed known lowPart interface bug |
>>>>>>> master
