using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class UpdatePageDirectoryCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Updates an existing page directory.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageDirectoryId { get; set; }

        [Required]
        [PositiveInteger]
        [Display(Name = "Parent directory", Description = "Choose the parent directory.")]
        public int ParentPageDirectoryId { get; set; }

        [Required]
        [StringLength(64)]
        [Display(Name = "Name", Description = "E.g. 'About the team' or 'Our products'")]
        public string Name { get; set; }

        [Required]
        [StringLength(64)]
        [Display(Name = "Url path", Description = "e.g. 'about-the-team' or 'products'.")]
        [Slug]
        public string UrlPath { get; set; }
    }
}
