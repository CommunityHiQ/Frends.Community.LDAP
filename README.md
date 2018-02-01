# Frends.Community.LDAP

FRENDS Community Task for Azure Blob related operations.

Task operations that use Azure DataMovement library for managing blobs.
https://github.com/Azure/azure-storage-net-data-movement

- [Installing](#installing)
- [Tasks](#tasks)
     - [UploadFileAsync](#uploadfileasync)
	 - [ListBlobs](#listblobs)
     - [DownloadBlobAsync](#downloadblobasync)
     - [ReadBlobContentAsync](#readblobcontentasync)
     - [DeleteBlobAsync](#deleteblobasync)
     - [DeleteContainerAsync](#deletecontainerasync)
- [Building](#building)
- [Contributing](#contributing)
- [Change Log](#change-log)

# Installing

You can install the task via FRENDS UI Task View or you can find the nuget package from the following nuget feed
'Insert nuget feed here'

# Tasks

## UploadFileAsync
Uploads file to target container. If the container doesn't exist, it will be created before the upload operation.

### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Source File | string | Full path to file that is uploaded. | 'c:\temp\uploadMe.xml' |
| Connection String | string | Connection string to Azure storage | 'UseDevelopmentStorage=true' |
| Container Name | string | Name of the azure blob storage container where the data will be uploaded. If the container doesn't exist, then it will be created. See [Naming and Referencing Containers](https://docs.microsoft.com/en-us/rest/api/storageservices/naming-and-referencing-containers--blobs--and-metadata) for naming conventions. | 'my-container' |
| Blob Type | enum: Append, Block or Page  | Azure blob type to upload. | Block |
| Rename To | string | If value is set, uploaded file will be renamed to this. | 'newFileName.xml' |
| Overwrite | bool | Should upload operation overwrite existing file with same name. | true |
| ParallelOperations | int | The number of the concurrent operations. | 64 |

### Returns

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| SourceFile | string | Full path of file uploaded | |
| Uri | string | Uri to uploaded blob | |

## ListBlobs
List blobs in container.

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Connection String | string | Connection string to Azure storage | 'UseDevelopmentStorage=true' |
| Container Name | string | Name of the azure blob storage container from where the data will be downloaded. | 'my-container' |
| Prefix | string | Blob prefix used while searching container | |

### Returns

Result is a list of object with following properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Name | string | Blob Name. With Directories this is empty. | |
| Uri | string | Blob Uri | |
| BlobType | string | Type of the blob. Either 'Block','Page' or 'Directory' | 'Block' |


## DownloadBlobAsync
Downloads blob to a file.

### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Connection String | string | Connection string to Azure storage | 'UseDevelopmentStorage=true' |
| Container Name | string | Name of the azure blob storage container from where the data will be downloaded. | 'my-container' |
| Blob Name | string | Name of the blob to be downloaded. | 'donwloadMe.xml' |
| Blob Type | enum: Append, Block or Page  | Azure blob type to download. | Block |
| Directory | string | Download destination directory. | 'c:\downloads' |
| FileExistsOperation | enum: Error, Rename, Overwrite | Action to take if destination file exists. Error: throws exception, Overwrite: writes over existing file, Rename: Renames file by adding '(1)' at the end (example: myFile.txt --> myFile(1).txt) | Error |

### Returns

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| FileName | string | Downloaded file name. | |
| Directory | string | Download directory. | |
| FullPath | string | Full path to downloaded file. | |

## ReadBlobContentAsync
Reads blob content to string.

### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Connection String | string | Connection string to Azure storage | 'UseDevelopmentStorage=true' |
| Container Name | string | Name of the azure blob storage container from where blob data is located. | 'my-container' |
| Blob Name | string | Name of the blob which content is read. | 'donwloadMe.xml' |
| Blob Type | enum: Append, Block or Page  | Azure blob type to read. | Block |
| Encoding Name | string | Encoding name in which blob content is read. | 'UTF-8' |

### Returns: 

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Content | string | Blob content. | |

## DeleteBlobAsync
Deletes a blob from target container. Operation result is seen as succesful even if the blob or container doesn't exist.

### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Connection String | string | Connection string to Azure storage | 'UseDevelopmentStorage=true' |
| Container Name | string | Name of the container where delete blob exists. | 'my-container' |
| Blob Name | string | Name of the blob to delete. | 'deleteMe.xml' |
| Blob Type | enum: Append, Block or Page  | Azure blob type to read. | Block |

### Returns: 

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Success | bool | Indicates whether the operation was succesful or not. | true |

## DeleteContainerAsync
Deletes a whole container from blob storage.

### Properties

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Connection String | string | Connection string to Azure storage | 'UseDevelopmentStorage=true' |
| Container Name | string | Name of the container to delete. | 'my-container' |

### Returns: 

| Property | Type | Description | Example |
| -------- | -------- | -------- | -------- |
| Success | bool | Indicates whether the operation was succesful or not. | true |

# Building

Clone a copy of the repo

`git clone https://github.com/CommunityHiQ/Frends.Community.Azure.Blob`

Restore dependencies

`nuget restore Frends.Community.Azure.Blob`

Rebuild the project

Run Tests with nunit3. Tests can be found under

`Frends.Community.Azure.Blob\bin\Release\Frends.Community.Azure.Blob.Tests.dll`

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
| 1.1.0 | Wrote documentation according to development quide lines. Added DownloadBlobAsync, ReadBlobContentAsync and ListBlobs tasks. |