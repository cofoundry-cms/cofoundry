using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Web
{
    public class RichTextWithMediaDataModel : IPageModuleDataModel
    {
        [Required, Display(Name = "Text")]
        [Html(HtmlToolbarPreset.BasicFormatting, HtmlToolbarPreset.Headings, HtmlToolbarPreset.Media)]
        //[Searchable]
        public string RawHtml { get; set; }

    }
}