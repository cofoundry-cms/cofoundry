using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the draft version of a page. If a draft version
    /// does not exist then one is created first.
    /// </summary>
    public class UpdatePageDraftVersionCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Id of the page to update the draft version for. A
        /// page can only have one drfat version.
        /// </summary>
        [PositiveInteger]
        [Required]
        public int PageId { get; set; }

        /// <summary>
        /// The descriptive human-readable title of the page that is often 
        /// used in the html page title meta tag. Does not have to be
        /// unique.
        /// </summary>
        [Display(Description = "A few words descriptive page title, e.g. 'About the team'. Google <a href=\"http://en.wikipedia.org/wiki/Search_engine_results_page\" target=\"_blank\">SERP</a> typically shows 50-60 characters")]
        [StringLength(300)]
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// The description of the content of the page. This is intended to
        /// be used in the description html meta tag.
        /// </summary>
        [Display(Name = "Meta description", Description = "Ideally 25-250 characters. The Google <a href=\"http://en.wikipedia.org/wiki/Search_engine_results_page\" target=\"_blank\">SERP</a> shows only the first 150 characters")]
        [StringLength(300)]
        public string MetaDescription { get; set; }

        /// <summary>
        /// Indicates whether the page should show in the auto-generated site map
        /// that gets presented to search engine robots.
        /// </summary>
        [Display(Name = "Show in site map?")]
        public bool ShowInSiteMap { get; set; }

        /// <summary>
        /// A title that can be used to share on social media via open 
        /// graph meta tags.
        /// </summary>
        [Display(Name = "Open graph title", Description = "Optional. The title that shows up when sharing the page on social media")]
        [StringLength(300)]
        public string OpenGraphTitle { get; set; }

        /// <summary>
        /// A description that can be used to share on social media via open 
        /// graph meta tags.
        /// </summary>
        [Display(Name = "Open graph description", Description = "Optional. The description that shows up when sharing the page on social media")]
        public string OpenGraphDescription { get; set; }

        /// <summary>
        /// An image that can be used to share on social media via open 
        /// graph meta tags.
        /// </summary>
        [Display(Name = "Open graph image", Description = "Optional. An image to show up when sharing the page on social media.")]
        [Image]
        [PositiveInteger]
        public int? OpenGraphImageId { get; set; }

        /// <summary>
        /// If set to true, the version will be published after it has been updated.
        /// </summary>
        public bool Publish { get; set; }

        /// <summary>
        /// Set a value to alter the publish date, otherwise the existing or current date is used.
        /// </summary>
        public DateTime? PublishDate { get; set; }
    }
}
