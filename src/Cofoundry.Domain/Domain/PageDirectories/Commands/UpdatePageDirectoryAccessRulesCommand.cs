using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates all access rules associated with a page.
    /// </summary>
    public class UpdatePageDirectoryAccessRulesCommand : UpdateAccessRulesCommandBase<UpdatePageDirectoryAccessRulesCommand.AddOrUpdatePageDirectoryAccessRuleCommand>, ILoggableCommand
    {
        /// <summary>
        /// Id of the directory to update.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// An instruction to either add or update an existing access rule
        /// attachd to a page directory.
        /// </summary>
        /// <inheritdoc/>
        public class AddOrUpdatePageDirectoryAccessRuleCommand : AddOrUpdateAccessRuleCommandBase
        {
            /// <summary>
            /// The Id of the access rule to update. If this is a new access
            /// rule, then this should be null.
            /// </summary>
            [PositiveInteger]
            public int? PageDirectoryAccessRuleId { get; set; }

            public override int? GetId()
            {
                return PageDirectoryAccessRuleId;
            }
        }
    }
}
