using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Updates an existing page access rule.
    /// </summary>
    public class UpdatePageAccessRuleCommand : ICommand, ILoggableCommand, IValidatableObject
    {
        /// <summary>
        /// Id of the access rule to update.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageAccessRuleId { get; set; }

        /// <summary>
        /// Unique 3 character code representing the user area to restrict
        /// the directory to. This cannot be the Cofoundry admin user area, as 
        /// access rules do not apply to admin panel users.
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
