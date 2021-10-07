using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates an existing page directory access rule.
    /// </summary>
    public class UpdatePageDirectoryAccessRuleCommand : ICommand, ILoggableCommand, IValidatableObject
    {
        /// <summary>
        /// Id of the access rule to update.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageDirectoryAccessRuleId { get; set; }

        /// <summary>
        /// Unique 6 character code representing the user area to restrict
        /// the page to.
        /// </summary>
        [Required]
        [MaxLength(3)]
        public string UserAreaCode { get; set; }

        /// <summary>
        /// Optionally restrict access to a specific role within the selected 
        /// user area.
        /// </summary>
        public int? RoleId { get; set; }

        /// <summary>
        /// The action to take when the rule is violated.
        /// </summary>
        [EnumDataType(typeof(RouteAccessRuleViolationAction))]
        public RouteAccessRuleViolationAction ViolationAction { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserAreaCode == CofoundryAdminUserArea.AreaCode)
            {
                yield return new ValidationResult("Access rules cannot be added for the Cofoundry admin user area because access rules do not apply to these users.", new string[] { nameof(UserAreaCode) });
            }
        }
    }
}
