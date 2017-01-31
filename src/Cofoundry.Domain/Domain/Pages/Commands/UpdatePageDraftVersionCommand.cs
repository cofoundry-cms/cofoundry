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
    public class UpdatePageDraftVersionCommand : ICommand, ILoggableCommand
    {
        [PositiveInteger]
        [Required]
        public int PageId { get; set; }

        [Display(Description = "A few words descriptive page title, e.g. 'About the team'. Google <a href=\"http://en.wikipedia.org/wiki/Search_engine_results_page\" target=\"_blank\">SERP</a> typically shows 50-60 characters")]
        [StringLength(300)]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Meta description", Description = "Ideally 25-250 characters. The Google <a href=\"http://en.wikipedia.org/wiki/Search_engine_results_page\" target=\"_blank\">SERP</a> shows only the first 150 characters")]
        [StringLength(300)]
        public string MetaDescription { get; set; }

        [Display(Name = "Show in site map?")]
        public bool ShowInSiteMap { get; set; }

        [Display(Name = "Open graph title", Description = "Optional. The title that shows up when sharing the page on social media")]
        [StringLength(300)]
        public string OpenGraphTitle { get; set; }

        [Display(Name = "Open graph description", Description = "Optional. The description that shows up when sharing the page on social media")]
        public string OpenGraphDescription { get; set; }

        [Display(Name = "Open graph image", Description = "Optional. An image to show up when sharing the page on social media.")]
        [Image]
        [PositiveInteger]
        public int? OpenGraphImageId { get; set; }

        public bool Publish { get; set; }
    }
}
