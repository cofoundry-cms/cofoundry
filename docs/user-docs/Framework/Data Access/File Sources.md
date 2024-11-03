Commands such as `AddImageAssetCommand` and `UpdateDocumentAssetCommand` allow you to upload files to Cofoundry by passing in an instance of a class that implements `IFileSource`. There are several `IFileSource` built into Cofoundry:

- [FormFileSource](#FormFileSource): For uploading a file from an ASP.NET `IFormFile` source.
- [EmbeddedResourceFileSource](#EmbeddedResourcefileSource): For uploading a file from an embedded resource.
- [StreamFileSource](#StreamFileSource): A general purpose `IFileSource` for working with streams.

## FormFileSource

Use ` Cofoundry.Web.FormFileSource` to wrap an ASP.NET `IFormFile` from an HTTP request:

```csharp
public async Task SaveFile(string title, IFormFile formFile)
{
    var imageAssetId = await _advancedContentRepository
        .ImageAssets()
        .AddAsync(new()
        {
            Title = title,
            File = new FormFileSource(formFile)
        });
}
```

## EmbeddedResourceFileSource

Use `EmbeddedResourceFileSource` to reference an embedded resource in your application. This is useful for testing or for initializing data:

```csharp
public async Task SaveFile()
{
    var fileSource = new EmbeddedResourceFileSource(this.GetType().Assembly, "MyProject.MyNamespace.MyFolder", "myimage.jpg");
    var imageAssetId = await _advancedContentRepository
        .ImageAssets()
        .AddAsync(new()
        {
            Title = "My Embedded Image",
            File = fileSource
        });
}
```

## StreamFileSource

`StreamFileSource` is a general purpose `IFileSource` that gives you complete control over how the stream is created and returned.

```csharp
public async Task SaveFile()
{
    var filePath = "c:\\my-path\\myimage.jpg";
    var fileName = Path.GetFileName(filePath);
    var fileSource = new StreamFileSource(fileName, () => File.OpenRead(filePath));

    var imageAssetId = await _advancedContentRepository
        .ImageAssets()
        .AddAsync(new()
        {
            Title = Path.GetFileNameWithoutExtension(fileName),
            File = fileSource
        });
}
```