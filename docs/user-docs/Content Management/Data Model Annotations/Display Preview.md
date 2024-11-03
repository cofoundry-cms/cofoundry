Preview annotations can be used to enhance the way data models are displayed in certain scenarios. 

An example of this is when displaying custom entities in grids where by default the `Title` property is displayed to identify the item, however we could enhance the listing by including an image or other field as a description.

Nested data models can be even more difficult to show in a grid, because they don't necessarily have a `Title` field. 

In these scenarios we can use any of the following data annotations to describe how the data should be displayed:

- **`[PreviewImage]`:** Annotate an image id property to include it as the first column of the grid.
- **`[PreviewTitle]`:**  Annotate a string field to use it as the title/identity field.
- **`[PreviewDescription]`:** Annotate a string field to use it as an additional description column

#### Example

This example is taken from the Carousel example in the [PageBlockTypes sample](https://github.com/cofoundry-cms/Cofoundry.Samples.PageBlockTypes):

```csharp
public class CarouselSlideDataModel : INestedDataModel
{
    [PreviewImage]
    [Display(Description = "Image to display as the background to the slide.")]
    [Required]
    [Image]
    public int ImageId { get; set; }

    [PreviewTitle]
    [Required]
    [Display(Description ="Title to display in the slide.")]
    [MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [Display(Description ="Formatted text to display in the slide.")]
    [Required]
    [Html(HtmlToolbarPreset.BasicFormatting)]
    public string Text { get; set; } = string.Empty;
}
```

This is how the carousel slide model displays in the admin panel:

![test](https://www.cofoundry.org/assets/images/9-2618524136981-620407/9-editing-with-preview-attributes.png?width=600)