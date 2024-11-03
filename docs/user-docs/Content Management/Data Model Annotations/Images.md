There are two different types of image data model annotations:

- [`[Image]`](#image)
- [`[ImageCollection]`](#imagecollection)

Each of these are explained below:

## [Image]

The `[Image]` data annotation can be used to decorate an integer to indicate this is for the id of an image asset. 

A nullable integer indicates this is an optional field, while a non-null integer indicates this is a required field. 

#### Optional parameters

- **Tags:** Filters the image selection to only show images with tags that match this value.
- **Width:** Filters the image selection to only show items with a width exactly the same as this value.
- **Height:** Filters the image selection to only show items with a height exactly the same as this value.
- **MinWidth:** Filters the image selection to only show items with a width the same or greater than this value.
- **MinHeight:** Filters the image selection to only show items with a height  the same or greater than this value.
- **PreviewWidth:** The width to use when previewing the image in the admin panel. This is useful if you want to preview the image in a specific crop ratio, without restricting the size of images uploaded.
- **PreviewHeight:** The height to use when previewing the image in the admin panel. This is useful if you want to preview the image in a specific crop ratio, without restricting the size of images uploaded.

#### Example

```csharp
public class ExampleDataModel : ICustomEntityDataModel
{
    /// <summary>
    /// A non nullable property indicated the image is required.
    /// </summary>
    [Image]
    public int ExampleRequiredImageId { get; set; }

    /// <summary>
    /// A nullable property indicates the image is optional.
    /// </summary>
    [Image]
    public int? ExampleOptionalImageId { get; set; }

    /// <summary>
    /// The image selector is filtered to images that are exactly 600 pixels wide, but
    /// can be any height.
    /// </summary>
    [Image(Width = 600)]
    public int? Example600WidthImageId { get; set; }

    /// <summary>
    /// The image selector is filtered to images that are exactly 400 x 400.
    /// </summary>
    [Image(Width = 400, Height = 400)]
    public int? Example400SquareImageId { get; set; }

    /// <summary>
    /// The image selector is filtered to images that are at least 800 x 600, but can
    /// be larger.
    /// </summary>
    [Image(MinWidth = 800, MinHeight = 600)]
    public int? ExampleMinDimensionsImageId { get; set; }

    /// <summary>
    /// Here we set the dimensions that the image is displayed
    /// at in the image selector. This is useful if you want to preview the 
    /// image at a specific crop ratio, e.g. a letterbox ratio for a banner.
    /// </summary>
    [Image(PreviewWidth = 1024, PreviewHeight = 480)]
    public int? ExampleWithPreviewRatioImageId { get; set; }

    /// <summary>
    /// This image selection is filtered to images labelled with these
    /// specific tags.
    /// </summary>
    [Image("cats", "dogs")]
    public int? ExampleTagImageId { get; set; }
}
```

## [ImageCollection]

The `[ImageCollection]` data annotation can be used to decorate a collection of integers, indicating the property represents a set of image asset ids. The editor allows for sorting and you can set filters for restricting file types.

#### Optional parameters

- **Tags:** Filters the image selection to only show images with tags that match this value.
- **Width:** Filters the image selection to only show items with a width exactly the same as this value.
- **Height:** Filters the image selection to only show items with a height exactly the same as this value.
- **MinWidth:** Filters the image selection to only show items with a width the same or greater than this value.
- **MinHeight:** Filters the image selection to only show items with a height  the same or greater than this value.

#### Example

```csharp
public class ExampleDataModel : ICustomEntityDataModel
{
    /// <summary>
    /// A non nullable property indicated the image is required.
    /// </summary>
    [ImageCollection]
    public int[] ImageIds { get; set; } = [];

    /// <summary>
    /// The image selector is filtered to images that are exactly 600 pixels wide, but
    /// can be any height.
    /// </summary>
    [ImageCollection(Width = 600)]
    public int[] FilteredTo600WidthImageIds { get; set; } = [];

    /// <summary>
    /// The image selector is filtered to images that are exactly 400 x 400.
    /// </summary>
    [ImageCollection(Width = 400, Height = 400)]
    public int[] FilteredTo400SquareImageIds { get; set; } = [];

    /// <summary>
    /// The image selector is filtered to images that are at least 800 x 600, but can
    /// be larger.
    /// </summary>
    [ImageCollection(MinWidth = 800, MinHeight = 600)]
    public int[] FilteredToMinDimensionsImageIds { get; set; } = [];
}
```