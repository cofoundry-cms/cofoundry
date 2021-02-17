using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Samples.Mail
{
    /// <summary>
    /// An example page block type. 
    /// See https://github.com/cofoundry-cms/cofoundry/wiki/Page-Block-Types
    /// for more information
    /// </summary>
    public class ContentSectionDataModel : IPageBlockTypeDataModel, IPageBlockTypeDisplayModel
    {
        [Display(Description = "Optional title to display at the top of the section")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Text", Description = "Rich text displayed at full width")]
        [Html(HtmlToolbarPreset.BasicFormatting, HtmlToolbarPreset.Headings, HtmlToolbarPreset.Media)]
        public string HtmlText { get; set; }
    }
}