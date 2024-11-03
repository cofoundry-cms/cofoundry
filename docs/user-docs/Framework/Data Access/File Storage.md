Cofoundry uses a storage abstraction when it needs to work with files. Most commonly this is used when saving or loading image or document assets, but the abstraction can also be used for other files.

To make use of this abstraction in your code you can request an instance of `IFileStoreService` from the DI container:

```csharp
using Cofoundry.Domain.Data;

public class FileStorageExample
{
    private readonly IFileStoreService _fileStoreService;

    public FileStorageExample(IFileStoreService fileStoreService)
    {
        _fileStoreService = fileStoreService;
    }

    public async Task SaveFile(IFileSource fileSource)
    {
        // container or top-level folder name for storing files of the same type
        var containerName = "example-container";

        // a unique filename: do not trust user input
        var fileName = Guid.NewGuid().ToString("N");

        using var stream = await fileSource.OpenReadStreamAsync();
        await _fileStoreService.CreateAsync(containerName, fileName, stream);
    }
}
```

## Configuring the default service (file system) 

The default implementation stores files to a file system. You can configure the root path in your settings file:

- **Cofoundry:FileSystemFileStorage:FileRoot** The directory root in which to store files such as images, documents and file caches. The default value is "~/App_Data/Files/". `IPathResolver` is used to resolve this path so by default you should be able to use application relative and absolute file paths.

e.g.

```json
{
  "Cofoundry": {
    "FileSystemFileStorage:FileRoot": "F:\\Files\\MySite"
  }
}
```

## Support for cloud storage via plugins

Support for cloud storage providers and other file systems is configured by installing [plugins](/plugins). Plugins include:

- [Cofoundry.Plugins.Azure](https://github.com/cofoundry-cms/Cofoundry.Plugins.Azure): This plugin contains an IFileStoreService implementation for Azure Blob Storage.

## Implementing a custom IFileStoreService

To use your own custom file storage implementation you will first need to create your own class that implements `IFileStoreService` and then [register it to override the base implementation](/framework/dependency-injection#overriding-registrations).
