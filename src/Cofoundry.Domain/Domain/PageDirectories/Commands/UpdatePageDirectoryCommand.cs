using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates the main properties of an existing page directory. To
    /// update properties that affect the route, use <see cref="UpdatePageDirectoryUrlCommand"/>.
    /// </summary>
    public class UpdatePageDirectoryCommand : IPatchableByIdCommand, ILoggableCommand
    {
        /// <summary>
        /// Database id of the page directory to update.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// User friendly display name of the directory.
        /// </summary>
        [StringLength(200)]
        [Display(Name = "Name", Description = "E.g. 'About the team' or 'Our products'")]
        public string Name { get; set; }
    }
}
