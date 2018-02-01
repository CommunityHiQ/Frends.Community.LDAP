# Frends.Community.LDAP

FRENDS Community Task for Active Directory related operations.

Task operations that use Active Directory library.
https://github.com/CommunityHiQ/Frends.Community.LDAP

- [Installing](#installing)
- [Tasks](#tasks)
     - [AD_UserExists](#ad_userexists)
	 - [AD_CreateUser](#ad_createUser)
     - [AD_UpdateUser](#ad_updateUser)
     - [AD_AddGroups](#ad_addgroups)
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)

# Installing

You can install the task via FRENDS UI Task View or you can find the nuget package from the following nuget feed
'Insert nuget feed here'

# Tasks

## AD_UserExists
Searches Active Directory for user(s) specified by the given attribute and its value.

### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| LDAP uri | string | Actice Directory uri | 'example.fi' |
| Username | string | User name | 'user' |
| Password | string | Password | '****' |
| Authentication type | enum: ANone,Secure,Encryption,SecureSocketsLayer,ReadonlyServer,Anonymous,FastBind,Signing,Sealing,Delegation,ServerBind  | Authentication type | None |
| Attribute | string | Attribute name to be searched. |  |
| Value | string | Attribute value to be searched. |  |

### Returns

Result a object with parameters.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| operationSuccessful | bool | True, if user found | |
| user | DirectoryEntry(object) | Found user | |

## AD_CreateUser
Create a user to AD.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| LDAP uri | string | Actice Directory uri | 'example.fi' |
| Username | string | User name | 'user' |
| Password | string | Password | '****' |
| Authentication type | enum: ANone,Secure,Encryption,SecureSocketsLayer,ReadonlyServer,Anonymous,FastBind,Signing,Sealing,Delegation,ServerBind  | Authentication type | None |
| Cn | string | Common name | 'John Doe' |
| Ou | string | Organization unit, where the user is located. |  |
| Ad flags | List |  |  |
| Other attributes | List |  |  |

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
| LDAP uri | string | Actice Directory uri | 'example.fi' |
| Username | string | User name | 'user' |
| Password | string | Password | '****' |
| Authentication type | enum: ANone,Secure,Encryption,SecureSocketsLayer,ReadonlyServer,Anonymous,FastBind,Signing,Sealing,Delegation,ServerBind  | Authentication type | None |
| Cn | string | Common name | 'John Doe' |
| Ou | string | Organization unit, where the user is located. |  |
| Ad flags | List |  |  |
| Other attributes | List |  |  |

### Returns
Result a object with parameters.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| operationSuccessful | bool | True, if user found | |
| user | DirectoryEntry(object) | Updated user | |

## AD_AddGroups
Add the user in AD to group(s).

### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| LDAP uri | string | Actice Directory uri | 'example.fi' |
| Username | string | User name | 'user' |
| Password | string | Password | '****' |
| Authentication type | enum: ANone,Secure,Encryption,SecureSocketsLayer,ReadonlyServer,Anonymous,FastBind,Signing,Sealing,Delegation,ServerBind  | Authentication type | None |
| Cn | string | Common name | 'John Doe' |
| Ou | string | Organization unit, where the user is located. |  |
| Ad flags | List |  |  |
| Other attributes | List |  |  |

### Returns: 
Result a object with parameters.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| operationSuccessful | bool | True, if user found | |
| user | DirectoryEntry(object) | Added user | |

# Building

Clone a copy of the repo

`git clone https://github.com/CommunityHiQ/Frends.Community.LDAP`

Restore dependencies

`nuget restore FreFrends.Community.LDAP`

Rebuild the project

Run Tests with nunit3. Tests can be found under

`Frends.Community.Azure.Blob\bin\Release\Frends.Community.Azure.Blob.Tests.dll-bin\Release\Frends.Community.LDAPTests.dll`

Create a nuget package

`nuget pack nuspec/Frends.Community.Azure.Blob.nuspec`

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
| 1.0.0 | First version. Includes AD_UserExists, AD_CreateUser, AD_UpdateUser, AD_AddGrouts |