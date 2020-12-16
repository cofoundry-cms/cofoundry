using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.BasicTestSite
{
    public class TestOptionSource : IListOptionSource
    {
        public ICollection<ListOption> Create()
        {
            var options = new List<ListOption>();
            options.Add(new ListOption("Test1", 1));
            options.Add(new ListOption("Test2", 2));
            options.Add(new ListOption("Test3", 3));

            return options;
        }
    }

    public class TestApiOptionSource : IListOptionApiSource
    {
        public string Path => "/test-admin/api/pets";

        public string NameField => "title";

        public string ValueField => "id";
    }

    /// <summary>
    /// This defines the custom data that gets stored with each blog post. Data
    /// is stored in an unstructured format (json) so simple data types are 
    /// best. For associations, you just need to store the key of the relation.
    /// </summary>
    public class BlogPostDataModel : ICustomEntityDataModel
    {
        [MaxLength(1000)]
        [Required]
        [Display(Description = "A description for display in search results and in the details page meta description.")]
        [MultiLineText(Rows = 10)]
        public string ShortDescription { get; set; }

        [Image(MinWidth = 460, MinHeight = 460)]
        [Display(Name = "Thumbnail Image", Description = "Square image that displays against the blog in the listing page.")]
        public int ThumbnailImageAssetId { get; set; }

        [Required]
        [Display(Name = "Categories", Description = "Drag and drop to customize the category ordering.")]
        [CustomEntityCollection(CategoryCustomEntityDefinition.DefinitionCode, IsOrderable = true)]
        public ICollection<int> CategoryIds { get; set; }

        [Display(Name = "Category", Description = "Test Single Category.")]
        [CustomEntity(CategoryCustomEntityDefinition.DefinitionCode)]
        public int CategoryId { get; set; }

        //[Required]
        //[CheckboxList(typeof(TestOptionSource), NoValueText = "None")]
        //public ICollection<int> TestCheckboxList1 { get; set; }

        //[CheckboxList(typeof(PublishStatus))]
        //public ICollection<PublishStatus> TestCheckboxList2 { get; set; }

        [SelectList(typeof(TestApiOptionSource))]
        public ICollection<int> TestCheckboxList3 { get; set; }

        //[RadioList(typeof(TestOptionSource), DefaultItemText ="OffNot")]
        //public int TestOption1 { get; set; }

        //[RadioList(typeof(PublishStatus))]
        //public PublishStatus TestOption2 { get; set; }

        //[RadioList(typeof(TestApiOptionSource))]
        //public int TestOption3 { get; set; }

        //[RadioList(typeof(TestOptionSource), DefaultItemText = "Off")]
        //public int? TestNullableOption1 { get; set; }

        //[RadioList(typeof(PublishStatus))]
        //public PublishStatus? TestNullableOption2 { get; set; }

        //[RadioList(typeof(TestApiOptionSource))]
        //public int? TestNullableOption3 { get; set; }

        [Required]
        [ImageCollection]
        [Display(Name = "Images")]
        public ICollection<int> ThumbnailImageAssets{ get; set; }

        [Display(Name = "Html")]
        [Html(HtmlToolbarPreset.BasicFormatting, HtmlToolbarPreset.Headings, HtmlToolbarPreset.Media, HtmlToolbarPreset.Source)]
        public string TestHtml { get; set; }

        [Document]
        [Display(Name = "Document")]
        public int TestDocumentId{ get; set; }

        [Page]
        [Display(Name = "Page")]
        public int? PageId { get; set; }

        [PageDirectory]
        [Display(Name = "Page Directory")]
        public int PageDirectoryId { get; set; }

        [PageCollection]
        [Display(Name = "Page Collection Test")]
        public ICollection<int> PageIds { get; set; }

    }
}