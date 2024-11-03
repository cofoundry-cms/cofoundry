## Managing Document Assets

Document assets can be any non-media file that you might want to make available for download on your site e.g. pdfs, word docs or zip files.

The admin panel has a whole section devoted to managing documents where users can add, update and delete document assets from a central place.

## Document File Location

By default Cofoundry will save document files into the "App_Data" folder inside your project. See the [File Storage docs](/framework/data-access/file-storage) for more information on configuring file storage options.

## Generating Document URLs

### From a View or Template

The [Cofoundry View Helper](Cofoundry-View-Helper) is the best way to access this:

```html
@using Cofoundry.Web

@model MyContentDisplayModel
@inject ICofoundryHelper<MyContentDisplayModel> Cofoundry

/* From a model */
<img src="@Cofoundry.Routing.DocumentAsset(Model.MyDocumentAsset)">

/* From an id (see warning below) */
<img src="@await Cofoundry.Routing.DocumentAssetAsync(3)">
```

Warning: Generating a URL from just a document asset id involves getting more information about the document from the database. It is recommended that you try to include the full `DocumentAssetRenderDetails` object in your view model to take advantage of batch requests and async methods.

### From Code

You can request `IDocumentAssetRouteLibrary` from the DI container and use this to generate URLs. It is the same API used by the Cofoundry View Helper above.

```csharp
public class DocumentExample
{
    private readonly IDocumentAssetRouteLibrary _documentAssetRouteLibrary;

    public DocumentExample(IDocumentAssetRouteLibrary documentAssetRouteLibrary)
    {
        _documentAssetRouteLibrary = documentAssetRouteLibrary;
    }

    public string GetExampleUrl(IDocumentAssetRenderable document)
    {
        var url = _documentAssetRouteLibrary.DocumentAsset(document);

        return url;
    }
}
```

## Getting Document Data

The simplest way to get document data is by resolving an instance of `IContentRepositry` from the DI container.

```csharp
public class DocumentExample
{
    private readonly IDocumentAssetRouteLibrary _documentAssetRouteLibrary;
    private readonly IContentRepository _contentRepository;

    public DocumentExample(
        IDocumentAssetRouteLibrary documentAssetRouteLibrary,
        IContentRepository contentRepository
        )
    {
        _documentAssetRouteLibrary = documentAssetRouteLibrary;
        _contentRepository = contentRepository;
    }

    public async Task<string> GetExampleUrl(int documentId)
    {
        var document = await _contentRepository
            .DocumentAssets()
            .GetById(documentId)
            .AsRenderDetails()
            .ExecuteAsync();

        var url = _documentAssetRouteLibrary.DocumentAsset(document);

        return url;
    }
}
```

## Restricting File Types

By default Cofoundry validates uploaded files against a blocklist of potentially dangerous file extensions and mime types taken from [`DangerousFileConstants.cs`](https://github.com/cofoundry-cms/cofoundry/blob/master/src/Cofoundry.Core/Core/Constants/DangerousFileConstants.cs). 

This is not intended to be a bulletproof validation mechanism, but it does at least prevent a non-technical user accidentally uploading an executable or script file.

### Files with unknown MIME types

The `application/octet-stream` MIME type is blocked by default, this is the fall-back MIME type if one cannot be resolved from the file extension.

If you're finding that a file type is blocked, it may be because the MIME mapping is missing from the from underlying `FileExtensionContentTypeProvider`. In Cofoundry you can configure additional MIME mappings by creating a class that implements `IMimeTypeRegistration`, which will automatically be picked up and bootstrapped via the DI system:

```csharp
public class MyAdditionalMimeTypeRegistration : IMimeTypeRegistration
{
    public void Register(IMimeTypeRegistrationContext context)
    {
        context.AddOrUpdate(".epub", "application/epub+zip");
        context.AddOrUpdate(".csv", "text/csv");
    }
}
```

### Disabling file type restrictions

File type restrictions can be disabled using configuration settings. Note that these settings apply to both image and document asset files:

```json
{
  "Cofoundry:AssetFiles": {
    "FileExtensionValidation": "Disabled",
    "MimeTypeValidation": "Disabled"
  }
}
```

### Customizing file type restrictions

You can use `AssetFilesSettings` to fully configure the validation process. For either file extension or mime type validation you can choose to use either a blocklist or an allowlist, or disable validation completely.

This example shows a restrictive allowlist of file types:

```json
{
  "Cofoundry:AssetFiles": {
    "FileExtensionValidation": "UseAllowList",
    "MimeTypeValidation": "UseAllowList",
    "FileExtensionValidationList": [
      "png",
      "jpg",
      "jpeg",
      "gif",
      "pdf",
      "doc",
      "docx"
    ],
    "MimeTypeValidationList": [
      "image/png",
      "image/jpeg",
      "image/gif",
      "application/pdf",
      "application/msword",
      "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
    ]
  }
}
```

IANA keeps a full list of MIME types [here](http://www.iana.org/assignments/media-types/media-types.xhtml)