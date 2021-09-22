using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

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

        [OutputValue]
        public int OutputPageDirectoryId { get; set; }
    }
}
