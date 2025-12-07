using System.ComponentModel.DataAnnotations;

namespace Dev.Sandbox;

//public enum ColorOption
//{
//    Red,
//    Orange,
//    Yellow,
//    Green,
//    Blue,
//    Indigo,
//    Violet
//}

//public class ExampleDataModel : ICustomEntityDataModel
//{
//    [SelectList(typeof(ColorOption))]
//    public ColorOption FavoriteColor { get; set; }
//}

//public class ExampleListOptionSource : IListOptionSource
//{
//    public ICollection<ListOption> Create()
//    {
//        var options = new List<ListOption>();
//        options.Add(new ListOption("Negative", 1));
//        options.Add(new ListOption("Neutral", 2));
//        options.Add(new ListOption("Positive", 3));

//        return options;
//    }
//}

//public class TestOptionSource : IListOptionSource
//{
//    public IReadOnlyCollection<ListOption> Create()
//    {
//        var options = new List<ListOption>
//        {
//            new("Test1", 1),
//            new("Test2", 2),
//            new("Test3", 3)
//        };

//        return options;
//    }
//}

//public class ExampleDataModel : ICustomEntityDataModel
//{
//    [RadioList(typeof(ExampleListOptionSource), DefaultItemText = "Not Specified")]
//    public int? Feedback { get; set; }
//}

public class PetsApiOptionSource : IListOptionApiSource
{
    public string Path => "/test-admin/api/pets";

    public string NameField => "Title";

    public string ValueField => "Id";
}

//public class ExampleDataModel : ICustomEntityDataModel
//{
//    [CheckboxList(typeof(PetsApiOptionSource))]
//    public ICollection<int> YourPets { get; set; }
//}

[AttributeUsage(AttributeTargets.Property)]
public class HtmlWithCustomEditorAttribute : HtmlAttribute
{
    public HtmlWithCustomEditorAttribute()
        : base(HtmlToolbarPreset.BasicFormatting, HtmlToolbarPreset.Media, HtmlToolbarPreset.Source)
    {
        //ConfigFilePath = "/content/html-editor-config.json";
        Rows = 6;
    }
}

public class HeaderBlock : INestedDataModel
{
    [Required]
    public string Text { get; set; } = string.Empty;
}

public class TeaserBlockDataModel : INestedDataModel
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [PreviewImage]
    [Display(Name = "Image")]
    [Image]
    public int? ImageAssetId { get; set; }
}

[Display(Name = "Content Blocky block")]
public class ContentBlock : INestedDataModel
{
    [PreviewTitle]
    public string? Title { get; set; }

    [PreviewDescription]
    public string? Text { get; set; }
}

public class ExampleDataModel : ICustomEntityDataModel
{
    [NestedDataModelCollection(
        IsOrderable = true,
        MinItems = 2,
        MaxItems = 6)]
    public IReadOnlyCollection<TeaserBlockDataModel> Blocks { get; set; } = Array.Empty<TeaserBlockDataModel>();

    [NestedDataModelMultiTypeCollection(
        [typeof(ContentBlock), typeof(TeaserBlockDataModel), typeof(HeaderBlock)],
        IsOrderable = true,
        //MinItems = 2, 
        MaxItems = 6
        )]
    public IReadOnlyCollection<NestedDataModelMultiTypeItem> TestCollection { get; set; } = Array.Empty<NestedDataModelMultiTypeItem>();

    //[Required]
    //[CheckboxList(typeof(TestOptionSource), NoValueText = "None")]
    //public IReadOnlyCollection<int> TestCheckboxList1 { get; set; } = Array.Empty<int>();

    //[CheckboxList(typeof(PublishStatus))]
    //public IReadOnlyCollection<PublishStatus> TestCheckboxList2 { get; set; } = Array.Empty<PublishStatus>();

    //[CheckboxList(typeof(PetsApiOptionSource))]
    //public IReadOnlyCollection<int> TestCheckboxList3 { get; set; } = Array.Empty<int>();

    //[RadioList(typeof(TestOptionSource), DefaultItemText = "OffNot")]
    //public int TestRadioListOptionSource { get; set; }

    //[RadioList(typeof(PublishStatus))]
    //public PublishStatus TestRadioListEnumOption { get; set; }

    //[RadioList(typeof(PetsApiOptionSource))]
    //public int TestRadioListApiOption { get; set; }

    //[RadioList(typeof(TestOptionSource), DefaultItemText = "Off")]
    //public int? TestNullableOption1 { get; set; }

    //[RadioList(typeof(PublishStatus))]
    //public PublishStatus? TestNullableOption2 { get; set; }

    //[RadioList(typeof(PetsApiOptionSource))]
    //public int? TestNullableOption3 { get; set; }

    //[SelectList(typeof(PublishStatus))]
    //public PublishStatus? TestEnumSelectList { get; set; }

    //[SelectList(typeof(TestOptionSource))]
    //public int TestOptionSourceSelectList2 { get; set; }

    [SelectList(typeof(PetsApiOptionSource))]
    public int TestApiOptionSelectList { get; set; }

    //[Required]
    //[ImageCollection]
    //[Display(Name = "Images")]
    //public IReadOnlyCollection<int> ThumbnailImageAssets { get; set; } = Array.Empty<int>();

    //[Display(Name = "Html")]
    //[Html(HtmlToolbarPreset.BasicFormatting, HtmlToolbarPreset.Headings, HtmlToolbarPreset.Media, HtmlToolbarPreset.Source)]
    //public string? TestHtml { get; set; }

    //[Document]
    //[Display(Name = "Document")]
    //public int TestDocumentId { get; set; }

    //[Page]
    //[Display(Name = "Page")]
    //public int? PageId { get; set; }

    //[PageDirectory]
    //[Display(Name = "Page Directory")]
    //public int PageDirectoryId { get; set; }

    //[PageCollection]
    //[Display(Name = "Page Collection Test")]
    //public IReadOnlyCollection<int> PageIds { get; set; } = Array.Empty<int>();
}
