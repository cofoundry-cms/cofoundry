using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Adds a new page directory.
    /// </summary>
    public class AddPageDirectoryCommand : ICommand, ILoggableCommand
    {
        [Required]
        [PositiveInteger]
        [Display(Name = "Parent directory", Description = "Choose the parent under which to create this new directory.")]
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

        #region Output

        [OutputValue]
        public int OutputPageDirectoryId { get; set; }

        #endregion
    }
}
