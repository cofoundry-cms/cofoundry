using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Removes an access rule from a page.
    /// </summary>
    public class DeletePageAccessRuleCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Id of the access rule to delete.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageAccessRuleId { get; set; }
    }
}
