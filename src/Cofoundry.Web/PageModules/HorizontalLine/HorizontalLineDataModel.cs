using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// Data model representing a horizontal line indicating a thematic break in content
    /// </summary>
    public class HorizontalLineDataModel : IPageModuleDataModel, IPageModuleDisplayModel
    {
        [Display(Name = "Percentage width", Description = "Leave blank to use the default width.")]
        [Range(1, 100)]
        public int? PercentageWidth { get; set; }
    }
}