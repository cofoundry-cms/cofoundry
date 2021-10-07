using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Adds a new access rule to a page directory.
    /// </summary>
    public class AddPageDirectoryAccessRuleCommand : ICommand, ILoggableCommand, IValidatableObject
    {
        /// <summary>
        /// Id of the directory to add the rule to.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageDirectoryId { get; set; }

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

        /// <summary>
        /// Once the command has been executed, this will be set with the Id of the
        /// newly created record.
        /// </summary>
        [OutputValue]
        public int OutputPageDirectoryAccessRuleId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserAreaCode == CofoundryAdminUserArea.AreaCode)
            {
                yield return new ValidationResult("Access rules cannot be added for the Cofoundry admin user area because access rules do not apply to these users.", new string[] { nameof(UserAreaCode) });
            }
        }
    }
}
