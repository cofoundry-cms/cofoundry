using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class AddWebDirectoryCommand : ICommand, ILoggableCommand
    {
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

        #region Output

        [OutputValue]
        public int OutputWebDirectoryId { get; set; }

        #endregion
    }
}
