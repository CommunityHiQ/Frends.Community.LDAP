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
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)

# Installing

You can install the task via FRENDS UI Task View or you can find the nuget package from the following nuget feed
'Insert nuget feed here'

# Tasks

## AD_FetchObject
Searches Active Directory for object(s) specified by filter.

### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| LDAP uri | string | Actice Directory uri | 'LDAP://xx.xxx.xxx.xxx/CN=Users,DC=FRENDSTest01,DC=net' |
| Username | string | User name | 'user' |
| Password | string | Password | '****' |
| Authentication type | enum: None, Secure, Encryption, SecureSocketsLayer, ReadonlyServer, Anonymous, FastBind, Signing, Sealing, Delegation, ServerBind  | Authentication type https://msdn.microsoft.com/en-us/library/system.directoryservices.authenticationtypes(v=vs.110).aspx | None |
| filter | string | Attribute name to be searched. | '(&(objectClass=user)(sAMAccountName=TestAdmin))' |

### Returns

Result a object(s) with parameters.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| OutputObjectEntry | The collection of the DirectoryEntry | Found objects | |

https://msdn.microsoft.com/en-us/library/system.directoryservices.directoryentry(v=vs.110).aspx

Usage:
To fetch result use syntax:

#result[0].GetProperty("instanceType")

#result[0].GetPropertyLargeInteger("lastLogon") - If attribute is in integer8 format.

#result can be looped by loop element, also.

## AD_CreateUser
Create a user to AD.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| LDAP uri | string | Actice Directory uri | 'LDAP://xx.xxx.xxx.xxx' |
| Username | string | User name | 'user' |
| Password | string | Password | '****' |
| Authentication type | enum: None, Secure, Encryption, SecureSocketsLayer, ReadonlyServer, Anonymous, FastBind, Signing, Sealing, Delegation, ServerBind | Authentication type  https://msdn.microsoft.com/en-us/library/system.directoryservices.authenticationtypes(v=vs.110).aspx | None |
| Cn | string | Common name | 'MattiMeikalainen' |
| Ou | string | Organization unit, where the user is located. | CN=Users,DC=FRENDSTest01,DC=net |
| Ad flags | List | https://msdn.microsoft.com/en-us/library/ms680832(v=vs.85).aspx |  |
| Other attributes | List | parameters: Attribute=attribute name; Value: Value to be set; Data type: Attribute's type |  |

### Returns

Result a object with parameter.

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
| Authentication type | enum: None, Secure, Encryption, SecureSocketsLayer, ReadonlyServer, Anonymous, FastBind, Signing, Sealing, Delegation, ServerBind | Authentication type https://msdn.microsoft.com/en-us/library/system.directoryservices.authenticationtypes(v=vs.110).aspx | None |
| Cn | string | Common name | 'MattiMeikalainen' |
| Ou | string | Organization unit, where the user is located. | 'CN=Users,DC=FRENDSTest01,DC=net' |
| Ad flags | List | https://msdn.microsoft.com/en-us/library/ms680832(v=vs.85).aspx |  |
| Other attributes | List | parameters: Attribute=attribute name; Value: Value to be set(set to null if you want to clear the value); Data type: Attribute's type |  |

### Returns
Result a object with parameters.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| operationSuccessful | bool | True, if user found| |
| user | DirectoryEntry(object) | Updated user | |

## AD_AddGroups
Add the user in AD to group(s).

### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| LDAP uri | string | Actice Directory uri | 'LDAP://xx.xxx.xxx.xxx' |
| Username | string | User name | 'user' |
| Password | string | Password | '****' |
| Authentication type | enum: None, Secure, Encryption, SecureSocketsLayer, ReadonlyServer, Anonymous, FastBind, Signing, Sealing, Delegation, ServerBind | Authentication type https://msdn.microsoft.com/en-us/library/system.directoryservices.authenticationtypes(v=vs.110).aspx | None |
| Cn | string | Common name | 'CN=MattiMeikalainen' |
| Ou | string | Organization unit, where the user is located. | 'CN=Users,DC=FRENDSTest01,DC=net' |
| Ad flags | List | https://msdn.microsoft.com/en-us/library/ms680832(v=vs.85).aspx |  |
| AD_AddGroupsProperties| List | Groups the user to be added. | 'CN=Guests,CN=Builtin' |

### Returns: 
Result a object with parameters.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| operationSuccessful | bool | True, if operation is successful | |

# Building

Clone a copy of the repo

`git clone https://github.com/CommunityHiQ/Frends.Community.LDAP`

Restore dependencies

`nuget restore FreFrends.Community.LDAP`

Rebuild the project

Run Tests with nunit3. Tests can be found under

`Frends.Community.LDAP\bin\Release\Frends.Community.LDAPTests.dll`

Create a nuget package

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
| ----- | ----- |
| 1.0.0 | First version. Includes AD_FetchObjects, AD_CreateUser, AD_UpdateUser, AD_AddGroups |