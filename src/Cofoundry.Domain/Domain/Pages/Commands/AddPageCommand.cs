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
    public class AddPageCommand : ICommand, ILoggableCommand, IValidatableObject
    {
        [Display(Name = "Url path", Description = "Lower case and containing only letter, numbers, underscores and hyphens. E.g. 'about-the-team'")]
        [StringLength(70)]
        [Slug]
        public string UrlPath { get; set; }
        
        [Display(Name = "Web directory")]
        [Required(ErrorMessage = "Please choose a web directory")]
        [PositiveInteger]
        public int WebDirectoryId { get; set; }
        
        [Display(Name = "Market")]
        [PositiveInteger]
        public int? LocaleId { get; set; }
        
        [Required(ErrorMessage = "Please choose a page template")]
        [Display(Name = "Template")]
        [PositiveInteger]
        public int PageTemplateId { get; set; }

        [Display(Description = "A few words descriptive page title, e.g. 'About the team'. Google <a href=\"http://en.wikipedia.org/wiki/Search_engine_results_page\" target=\"_blank\">SERP</a> typically shows 50-60 characters")]
        [StringLength(300)]
        [Required]
        public string Title { get; set; }

        [Display(Name = "Tags", Description = "Separate each tag with a space: dog animal canine. Or to join 2 words together in one tag, use double quotes: \"golden retriever\". Used internally for searching for things.")]
        public string[] Tags { get; set; }

        [Display(Name = "Meta description", Description = "Ideally 25-250 characters. The Google <a href=\"http://en.wikipedia.org/wiki/Search_engine_results_page\" target=\"_blank\">SERP</a> shows only the first 150 characters")]
        [StringLength(300)]
        public string MetaDescription { get; set; }

        [Display(Name = "Show in site map?")]
        public bool ShowInSiteMap { get; set; }

        public bool Publish { get; set; }

        [Required]
        public PageType PageType { get; set; }

        [StringLength(70)]
        public string CustomEntityRoutingRule { get; set; }

        #region OpenGraph

        [Display(Name = "Open graph title", Description = "Optional. The title that shows up when sharing the page on social media")]
        [StringLength(300)]
        public string OpenGraphTitle { get; set; }

        [Display(Name = "Open graph description", Description = "Optional. The description that shows up when sharing the page on social media")]
        public string OpenGraphDescription { get; set; }

        [Display(Name = "Open graph image", Description = "Optional. An image to show up when sharing the page on social media.")]
        [Image]
        [PositiveInteger]
        public int? OpenGraphImageId { get; set; }

        #endregion

        #region Output

        [OutputValue]
        public int OutputPageId { get; set; }

        #endregion

        #region Custom Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PageType == PageType.CustomEntityDetails && string.IsNullOrWhiteSpace(CustomEntityRoutingRule))
            {
                yield return new ValidationResult("A routing rule is required for custom entity details page types.", new[] { "CustomEntityRoutingRule" });
            }
        }

        #endregion

    }
}
