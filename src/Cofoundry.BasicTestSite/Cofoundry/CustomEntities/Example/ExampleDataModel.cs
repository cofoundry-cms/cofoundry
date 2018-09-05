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
            Rows = 40;
        }
    }

    public class ExampleDataModel : ICustomEntityDataModel
    {
        [HtmlWithCustomEditor]
        public string Content { get; set; }

        [CustomEntity(CategoryCustomEntityDefinition.DefinitionCode)]
        public int CategoryId { get; set; }
    }
}