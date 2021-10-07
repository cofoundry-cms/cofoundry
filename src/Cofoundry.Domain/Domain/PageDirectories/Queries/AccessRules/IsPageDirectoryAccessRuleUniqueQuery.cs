using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Determines if a page directory  access rule already exists with the specified 
    /// rule configuration. 
    /// </summary>
    public class IsPageDirectoryAccessRuleUniqueQuery : IQuery<bool>, IValidatableObject
    {
        /// <summary>
        /// Id of a page directory access rule to exclude from the check. Used when 
        /// checking an existing rule for uniqueness.
        /// </summary>
        public int? PageDirectoryAccessRuleId { get; set; }

        /// <summary>
        /// Id of the directory that the uniqueness check should be scoped to.
        /// </summary>
        [Required]
        [PositiveInteger]
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// Unique 3 character code representing the user area to
        /// restrict access to.
        /// </summary>
        [Required]
        [MaxLength(3)]
        public string UserAreaCode { get; set; }

        /// <summary>
        /// Optional id of a role that the rule restricts page 
        /// access to. The role must belong to the user area defined by 
        /// <see cref="UserAreaCode"/>.
        /// </summary>
        public int? RoleId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserAreaCode == CofoundryAdminUserArea.AreaCode)
            {
                yield return new ValidationResult("Access rules do not apply to Cofoundry admin users.", new string[] { nameof(UserAreaCode) });
            }
        }
    }
}
