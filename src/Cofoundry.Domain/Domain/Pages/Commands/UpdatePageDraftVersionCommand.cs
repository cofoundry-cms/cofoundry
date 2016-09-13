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

        [Display(Name = "Page title", Description = "A few words descriptive page title, e.g. 'About the team'. Google <a href=\"http://en.wikipedia.org/wiki/Search_engine_results_page\" target=\"_blank\">SERP</a> shows a maximum of 66 characters")]
        [StringLength(70)]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Meta description", Description = "Ideally 25-250 characters. The Google <a href=\"http://en.wikipedia.org/wiki/Search_engine_results_page\" target=\"_blank\">SERP</a> shows only the first 150 characters")]
        [StringLength(256)]
        [Required]
        public string MetaDescription { get; set; }

        [Display(Name = "Meta keywords", Description = "Optional. A comma separated list of (ideally at least 3) keywords specific to this page e.g. 'dog,golden retriever,animal'. These will be combined with the site wide keywords.")]
        [StringLength(256)]
        public string MetaKeywords { get; set; }

        [Display(Name = "Show in site map?")]
        public bool ShowInSiteMap { get; set; }

        [Display(Name = "Open graph title", Description = "Optional. Use this title to override what Facebook sees as the title of the page")]
        [StringLength(64)]
        public string OpenGraphTitle { get; set; }

        [Display(Name = "Open graph description", Description = "Optional. This description shows up in Facebook")]
        public string OpenGraphDescription { get; set; }

        [Display(Name = "Open graph image", Description = "Optional. An image to show up within Facebook. The image must be at least 50px by 50px (though minimum 200px by 200px is preferred) and have a maximum aspect ratio of 3:1.")]
        [Image]
        [PositiveInteger]
        public int? OpenGraphImageId { get; set; }

        public bool Publish { get; set; }
    }
}
