using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Web
{
    /// <summary>
    /// Data model representing text entry with simple formatting and image/video media functionality
    /// </summary>
    public class RichTextWithMediaDataModel : IPageBlockTypeDataModel
    {
        [Required, Display(Name = "Text")]
        [Html(HtmlToolbarPreset.BasicFormatting, HtmlToolbarPreset.Headings, HtmlToolbarPreset.Media)]
        //[Searchable]
        public string RawHtml { get; set; }
    }
}