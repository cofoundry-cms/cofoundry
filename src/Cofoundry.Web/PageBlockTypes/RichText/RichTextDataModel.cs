using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Web
{
    /// <summary>
    /// Data model representing text entry with simple formatting like headings and lists
    /// </summary>
    public class RichTextDataModel : IPageBlockTypeDataModel
    {
        [Required, Display(Name = "Text")]
        [Html(HtmlToolbarPreset.BasicFormatting, HtmlToolbarPreset.Headings)]
        //[Searchable]
        public string RawHtml { get; set; }

    }
}