using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// Data model representing an embedded a Vimeo video 
    /// </summary>
    public class VimeoVideoDataModel : IPageModuleDataModel, IPageModuleDisplayModel
    {
        [Required]
        [Display(Name = "Vimeo video")]
        [Vimeo]
        public VimeoVideo Video { get; set; }
    }
}