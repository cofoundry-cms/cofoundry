A *Page Block Type* represents a type of content that can be inserted into a region of a *page template* such as *Image*, *Plain Text* or *Raw Html*. Cofoundry includes some common types built in, but you can easily create your own custom types to represent more bespoke concepts, this gives you fine grained control over the data and markup building blocks that end users can use to create content on your website.

## Built-in Page Block Types

- **Document:** A simple link to download a document asset
- **Image:** Displays a single image asset, optionally wrapped in a link
- **PlainText:** Multiple lines of simple text 
- **RawHtml:** Html text with full HTML editing
- **RichText:** Text entry with simple formatting like headings and lists
- **RichTextWithMedia:** Text entry with simple formatting and image/video media functionality
- **SingleLine:** A single line of text, no formatting

## Custom Page Block Types

Creating a basic page block type is pretty simple, it just requires two files, a data model and a view file. Additional features allow you to customize the display model mapping process and have more control over the model that is sent to the view file.

Cofoundry's built-in page block types are created using the same process, so as well as the examples listed here you can [view the code](https://github.com/cofoundry-cms/cofoundry/tree/master/src/Cofoundry.Web/PageBlockTypes) and use it as a reference, or we also have a [sample project](https://github.com/cofoundry-cms/Cofoundry.Samples.PageBlockTypes/) that is filled with page block type examples.

### Injecting ICofoundryBlockTypeHelper

You can take advantage of some additional block type functionality by injecting `ICofoundryPageBlockTypeHelper<TDisplayModel>` into your view. This is an enhanced version of the [Cofoundry view helper](Cofoundry-View-Helper) and is injected at the top of your view in the same way you would inject any service. When injecting the helper we recommend aliasing it as `Cofoundry`.

### Simple Example

In this simple example the data model has two text properties which use data annotations to help describe the data. Cofoundry uses this property data to automatically generate a data entry form so you don't have to.

**MyContentDataModel.cs**

```csharp
using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// The data model defines the raw data saved to the database. In this
/// simple scenario this class also represents the display model used
/// in the view
/// </summary>
public class MyContentDataModel : IPageBlockTypeDataModel, IPageBlockTypeDisplayModel
{
    [MaxLength(1000)]
    [Required]
    public string Title { get; set; } = string.Empty;

    [MultiLineText]
    public string? Description { get; set; }
}
```

**MyContent.cshtml**

```html
@using Cofoundry.Web

@model MyContentDataModel
@inject ICofoundryBlockTypeHelper<MyContentDataModel> Cofoundry

@Cofoundry.BlockType.UseDescription("This is the description that shows up in the page block type selection UI")

<h2>@Model.Title</h2>
<p>@Model.Description</p>
```

### File Location

- The default convention is to place block types in *'Cofoundry\PageBlockTypes'* and then each individual block type inside a child folder named using the block type name e.g. 'Cofoundry\PageBlockTypes\ProductCarousel\'
- Additionally the root folder can also be *'Views\PageBlockTypes'* or simply *'PageBlockTypes'* in the root of the application
- You can add other custom locations by implementing a class that inherits from `IPageBlockTypeViewLocationRegistration`

### Registration

Like *Page Templates*, *Page Block Types* are automatically scanned and added at startup.

### DataModel DataAnnotations

Most existing data annotations like `[Required]`, `[MaxLength(10)]` and `[Display(Name="Title")]` will work as expected, but there are also Cofoundry specific annotations like `[Date]`, `[Image]` and `[CustomEntityCollection]`. You can also create your own, or implement an `IModelMetaDataDecorator` to add additional metadata to an existing attribute.

For more information see [Data Model Annotations](Data-Model-Annotations)

### Customizing the Display Model

Occasionally you might want to include extra data in your view or transform your data model in some way before rendering it. This is particularly important when including references to other entities like images and you want to include them in your display model.

To allow for this Cofoundry lets you define a separate display model and mapping class that handles the transformation. Let's beef up our simple example with a couple of extra properties and create a new display model:

**MyContentDataModel.cs**

```csharp
using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// The data model defines the raw data saved to the database.
/// </summary>
public class MyContentDataModel : IPageBlockTypeDataModel
{
    [MaxLength(1000)]
    [Required]
    public string Title { get; set; } = string.Empty;

    [Html(HtmlToolbarPreset.BasicFormatting)]
    public string? Description { get; set; }

    [Image]
    public int ThumbnailImageAssetId { get; set; }
}
```

**MyContentDisplayModel.cs**

```csharp
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;

/// <summary>
/// The display model is the model that is passed through to the view
/// </summary>
public class MyContentDisplayModel : IPageBlockTypeDisplayModel
{
    public string Title { get; set; }

    public IHtmlContent Description { get; set; }

    public ImageAssetRenderDetails ThumbnailImageAsset { get; set; }
}
```

**MyContentDisplayModelMapper.cs**

```csharp
using Cofoundry.Domain;
using Cofoundry.Core;

/// <summary>
/// The mapper supports DI which gives you flexibility in what data
/// you want to include in the display model and how you want to 
/// map it. Mapping is done in batch to improve performance when 
/// the same module type is used multiple times on a page.
/// </summary>
public class MyContentDisplayModelMapper : IPageBlockTypeDisplayModelMapper<MyContentDataModel>
{
    private readonly IContentRepository _contentRepository;

    public MyContentDisplayModelMapper(
        IContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    public async Task MapAsync(
            PageBlockTypeDisplayModelMapperContext<MyContentDataModel> context,
            PageBlockTypeDisplayModelMapperResult<MyContentDataModel> result
        )
    {
        var imageAssetIds = context.Items.SelectDistinctModelValuesWithoutEmpty(i => i.ThumbnailImageAssetId);
        var imageAssets = await _contentRepository
            .WithContext(context.ExecutionContext)
            .ImageAssets()
            .GetByIdRange(imageAssetIds)
            .AsRenderDetails()
            .ExecuteAsync();

        foreach (var item in context.Items)
        {
            var displayModel = new MyContentDisplayModel
            {
                Title = item.DataModel.Title,
                Description = new HtmlString(item.DataModel.Description),
                ThumbnailImageAsset = imageAssets.GetValueOrDefault(item.DataModel.ThumbnailImageAssetId)
            };

            result.Add(item, displayModel);
        }
    }
}
```

**MyContent.cshtml**

```html
@using Cofoundry.Domain
@using Cofoundry.Web

@model MyContentDisplayModel
@inject ICofoundryBlockTypeHelper<MyContentDisplayModel> Cofoundry

@Cofoundry.BlockType.UseDescription("This is the description that shows up in the page block type selection UI")

<img src="@Cofoundry.Routing.ImageAsset(Model.ThumbnailImageAsset)">
<h2>@Model.Title</h2>
@Cofoundry.Sanitizer.Sanitize(Model.Description)
```

## Enhancing your display model

### List context

Implement `IListablePageBlockTypeDisplayModel` on your display model if your module will appear in a list. This provides you with more information about where the module is positioned in the list.

### Page context

Implement `IPageBlockWithParentPageData` on your display model to get access to the parent page model from within your block. The `ParentPage` property will be set prior to rendering.

## What's Happening Under The Hood

Whilst most Cofoundry entities such as Pages, Users and Images are backed by *structured data* (relational SQL tables), page block type data is stored as *unstructured data* (serialized JSON). This helps us strike a balance between speed, integrity and flexibility. For page block types we sacrifice a little of this speed and data integrity to provide super flexibility, but we do have some measures in place to mitigate the downsides such as caching, batch lookups and dependency logging.


