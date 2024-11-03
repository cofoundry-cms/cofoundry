# Cofoundry.Plugins.Azure

[![Build status](https://ci.appveyor.com/api/projects/status/65bx24r2ugb1hoko?svg=true)](https://ci.appveyor.com/project/Cofoundry/cofoundry-plugins-azure)
[![NuGet](https://img.shields.io/nuget/v/Cofoundry.Plugins.Azure.svg)](https://www.nuget.org/packages/Cofoundry.Plugins.Azure/)


This library is a plugin for [Cofoundry](https://www.cofoundry.org/). For more information on getting started with Cofoundry check out the [Cofoundry repository](https://github.com/cofoundry-cms/cofoundry).

## Overview

This library contains services, abstractions and helpers for running in an Azure environment. Principally this consists of:

- **AzureBlobFileService:** IFileStoreService for azure blog storage

## Settings

- **Cofoundry:Plugins:Azure:BlobStorageConnectionString** The connection string to use when accessing files in blob storage.
- **Cofoundry:Plugins:Azure:Disabled:** Indicates whether the plugin should be disabled, which means services will not be bootstrapped. Disable this in dev when you want to run using the standard non-cloud services. Defaults to false.





