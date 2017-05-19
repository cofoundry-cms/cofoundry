using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.BasicTestSite
{
    /// <summary>
    /// This defines the custom data that gets stored with each
    /// category.
    /// </summary>
    public class CategoryDataModel : ICustomEntityVersionDataModel
    {
        [MaxLength(500)]
        [Display(Description = "A short description that appears as a tooltip when hovering over the category.")]
        [MultiLineText]
        public string ShortDescription { get; set; }
    }
}