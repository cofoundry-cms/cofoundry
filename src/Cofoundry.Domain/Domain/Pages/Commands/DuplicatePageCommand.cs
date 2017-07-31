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
    public class DuplicatePageCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int PageToDuplicateId { get; set; }

        [Display(Name = "Url path", Description = "Lower case and containing only letter, numbers, underscores and hyphens. E.g. 'about-the-team'")]
        [StringLength(64)]
        [Slug]
        public virtual string UrlPath { get; set; }

        [StringLength(70)]
        public string CustomEntityRoutingRule { get; set; }

        [Display(Name = "Directory")]
        [Required(ErrorMessage = "Please choose a directory")]
        [PositiveInteger]
        public int PageDirectoryId { get; set; }
        
        [Display(Name = "Market")]
        [PositiveInteger]
        public int? LocaleId { get; set; }

        [Display(Description = "A few words descriptive page title, e.g. 'About the team'. Google <a href=\"http://en.wikipedia.org/wiki/Search_engine_results_page\" target=\"_blank\">SERP</a> shows a maximum of 66 characters")]
        [StringLength(70)]
        [Required]
        public string Title { get; set; }

        #region Output

        [OutputValue]
        public int OutputPageId { get; set; }

        #endregion
    }
}
