using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.BasicTestSite
{
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

    //public class ExampleDataModel : ICustomEntityDataModel
    //{
    //    [RadioList(typeof(ExampleListOptionSource), DefaultItemText = "Not Specified")]
    //    public int? Feedback { get; set; }
    //}

    //public class PetsApiOptionSource : IListOptionApiSource
    //{
    //    public string Path => "/admin/api/test";

    //    public string NameField => "Title";

    //    public string ValueField => "Id";
    //}


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
        public string Text { get; set; }
    }

    public class TeaserBlockDataModel : INestedDataModel
    {
        [Required]
        public string Title { get; set; }

        [PreviewImage]
        [Display(Name = "Image")]
        [Image]
        public int? ImageAssetId { get; set; }
    }

    [Display(Name ="Content Blocky block")]
    public class ContentBlock : INestedDataModel
    {
        [PreviewTitle]
        public string Title { get; set; }

        [PreviewDescription]
        public string Text { get; set; }
    }

    public class ExampleDataModel : ICustomEntityDataModel
    {
        [NestedDataModelCollection(
            IsOrderable = true,
            MinItems = 2,
            MaxItems = 6)]
        public ICollection<TeaserBlockDataModel> Blocks { get; set; }

        [NestedDataModelMultiTypeCollection(
            new Type[] { typeof(ContentBlock), typeof(TeaserBlockDataModel), typeof(HeaderBlock) },
            IsOrderable = true,
            //MinItems = 2, 
            MaxItems = 6
            )]
        public ICollection<NestedDataModelMultiTypeItem> TestCollection { get; set; }
    }
}