using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Removes an access rule from a page directory.
    /// </summary>
    public class DeletePageDirectoryAccessRuleCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Id of the access rule to delete.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageDirectoryAccessRuleId { get; set; }
    }
}
