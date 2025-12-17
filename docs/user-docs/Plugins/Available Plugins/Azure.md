This library contains services, abstractions and helpers for running in an Azure environment. Principally this consists of:

- **AzureBlobFileService:** IFileStoreService implementation that uses azure blob storage

## Installation

Install the [Cofoundry.Plugins.Azure](https://www.nuget.org/packages/Cofoundry.Plugins.Azure/) package via Nuget, e.g. via the CLI:

```bash
dotnet add package Cofoundry.Plugins.Azure
```

## Configuration

- **Cofoundry:Plugins:Azure:BlobStorageConnectionString** The connection string to use when accessing files in blob storage.
- **Cofoundry:Plugins:Azure:Disabled:** Indicates whether the plugin should be disabled, which means services will not be bootstrapped. Disable this in dev when you want to run using the standard non-cloud services. Defaults to false.
