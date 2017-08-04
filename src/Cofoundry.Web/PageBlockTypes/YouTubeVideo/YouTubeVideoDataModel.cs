using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// Data model representing an embedded YouTube video
    /// </summary>
    public class YoutubeVideoDataModel : IPageBlockTypeDataModel, IPageBlockTypeDisplayModel
    {
        [Required]
        [Display(Name = "Video Id")]
        public string VideoId { get; set; }

        [Required]
        [Display(Description = "A short title about the video")]
        public string Title { get; set; }

        [Required]
        [Display(Description = "Enter a description of the video")]
        public string Description { get; set; }
    }
}