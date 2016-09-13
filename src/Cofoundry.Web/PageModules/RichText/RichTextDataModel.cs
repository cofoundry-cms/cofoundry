using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Web
{
    public class RichTextDataModel : IPageModuleDataModel
    {
        [Required, Display(Name = "Rich Text")]
        [AllowHtml]
        [Html(HtmlToolbarPreset.BasicFormatting, HtmlToolbarPreset.Headings)]
        //[Searchable]
        public string RawHtml { get; set; }

    }
}