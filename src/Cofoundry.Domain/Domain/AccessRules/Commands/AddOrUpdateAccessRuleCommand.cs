using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{

    /// <summary>
    /// An instruction to either add or update an existing access rule
    /// attachd to an entity. Used in commands that inherit from 
    /// <see cref="UpdateAccessRulesCommandBase"/>.
    /// </summary>
    public abstract class AddOrUpdateAccessRuleCommandBase : IValidatableObject
    {
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
        /// Gets the database primary key of the access rule. Can be <see langword="null"/>
        /// if this instance represents a new rule.
        /// </summary>
        public abstract int? GetId();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserAreaCode == CofoundryAdminUserArea.AreaCode)
            {
                yield return new ValidationResult("Access rules cannot be added for the Cofoundry admin user area because access rules do not apply to these users.", new string[] { nameof(UserAreaCode) });
            }
        }
    }
}
