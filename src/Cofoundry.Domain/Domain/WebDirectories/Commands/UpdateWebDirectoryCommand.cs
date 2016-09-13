using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class UpdateWebDirectoryCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        public int WebDirectoryId { get; set; }

        [Required]
        [PositiveInteger]
        [Display(Name = "Parent directory", Description = "Choose the parent under which to create this new directory.")]
        public int ParentWebDirectoryId { get; set; }

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
