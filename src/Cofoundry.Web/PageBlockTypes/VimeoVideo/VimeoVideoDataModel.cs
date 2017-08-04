using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// Data model representing an embedded a Vimeo video 
    /// </summary>
    public class VimeoVideoDataModel : IPageBlockTypeDataModel, IPageBlockTypeDisplayModel
    {
        [Required]
        [Display(Name = "Vimeo video")]
        [Vimeo]
        public VimeoVideo Video { get; set; }
    }
}