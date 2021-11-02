using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates all access rules associated with a page.
    /// </summary>
    public class UpdatePageAccessRulesCommand : UpdateAccessRulesCommandBase<UpdatePageAccessRulesCommand.AddOrUpdatePageAccessRuleCommand>, ILoggableCommand
    {
        /// <summary>
        /// Id of the page to update.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageId { get; set; }

        /// <summary>
        /// An instruction to either add or update an existing access rule
        /// attachd to a page.
        /// </summary>
        /// <inheritdoc/>
        public class AddOrUpdatePageAccessRuleCommand : AddOrUpdateAccessRuleCommandBase
        {
            /// <summary>
            /// The Id of the access rule to update. If this is a new access
            /// rule, then this should be null.
            /// </summary>
            [PositiveInteger]
            public int? PageAccessRuleId { get; set; }

            public override int? GetId()
            {
                return PageAccessRuleId;
            }
        }
    }

}
