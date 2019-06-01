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
    /// Creates a new page, copying from an existing page.
    /// </summary>
    public class DuplicatePageCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Id of the existing page to copy from.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageToDuplicateId { get; set; }

        /// <summary>
        /// The path of the new page within the directory. This must be
        /// unique within the directory the page is parented to.
        /// E.g. 'about-the-team'
        /// </summary>
        [Display(Name = "Url path", Description = "Lower case and containing only letter, numbers, underscores and hyphens. E.g. 'about-the-team'")]
        [StringLength(64)]
        [Slug]
        public virtual string UrlPath { get; set; }

        /// <summary>
        /// If copying a CustomEntityDetails page, this will need to be set
        /// to a value that matches the RouteFormat of an existing
        /// ICustomEntityRoutingRule e.g. "{Id}/{UrlSlug}".
        /// </summary>
        [StringLength(70)]
        public string CustomEntityRoutingRule { get; set; }

        /// <summary>
        /// The id of the directory the new page should be added to.
        /// </summary>
        [Display(Name = "Directory")]
        [Required(ErrorMessage = "Please choose a directory")]
        [PositiveInteger]
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// Optional id of the locale if used in a localized site.
        /// </summary>
        [Display(Name = "Locale")]
        [PositiveInteger]
        public int? LocaleId { get; set; }

        /// <summary>
        /// The descriptive human-readable title of the page that is often 
        /// used in the html page title meta tag. Does not have to be
        /// unique.
        /// </summary>
        [Display(Description = "A few words descriptive page title, e.g. 'About the team'. Google <a href=\"http://en.wikipedia.org/wiki/Search_engine_results_page\" target=\"_blank\">SERP</a> shows a maximum of 66 characters")]
        [StringLength(70)]
        [Required]
        public string Title { get; set; }

        #region Output

        /// <summary>
        /// The database id of the newly created page. This is set after the 
        /// command has been run.
        /// </summary>
        [OutputValue]
        public int OutputPageId { get; set; }

        #endregion
    }
}
