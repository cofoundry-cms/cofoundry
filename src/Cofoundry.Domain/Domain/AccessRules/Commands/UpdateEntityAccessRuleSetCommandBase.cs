using Cofoundry.Core;
using Cofoundry.Core.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This base class encapsulated functionality for updating access rules
    /// attached to an entity.
    /// </summary>
    public abstract class UpdateAccessRuleSetCommandBase<TAddOrUpdateAccessRuleCommand> : IValidatableObject
        where TAddOrUpdateAccessRuleCommand : AddOrUpdateAccessRuleCommandBase
    {
        /// <summary>
        /// A collection of access rules representing the expected state after
        /// the command has been completed. Any rules that are not included in the 
        /// collection are removed from the database.
        /// </summary>
        [ValidateObject]
        public ICollection<TAddOrUpdateAccessRuleCommand> AccessRules { get; set; } = new List<TAddOrUpdateAccessRuleCommand>();

        /// <summary>
        /// Unique 3 character code representing the <see cref="UserArea"/> with
        /// the sign in page to redirect to when a user does not meet the criteria of 
        /// the access rules directly associated with this page.
        /// </summary>
        public string UserAreaCodeForSignInRedirect { get; set; }

        /// <summary>
        /// The action that should be taken when a user does not meet the criteria 
        /// of the access rules directly associated with the page.
        /// </summary>
        [EnumDataType(typeof(AccessRuleViolationAction))]
        public AccessRuleViolationAction ViolationAction { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var accessRules = EnumerableHelper.Enumerate(AccessRules);

            if (!string.IsNullOrEmpty(UserAreaCodeForSignInRedirect)
                && !accessRules.Any(r => r.UserAreaCode == UserAreaCodeForSignInRedirect))
            {
                yield return new ValidationResult("You can only redirect to the sign in page for a user area that appears in the access rules.", new string[] { nameof(UserAreaCodeForSignInRedirect) });
            }
            else if (UserAreaCodeForSignInRedirect == CofoundryAdminUserArea.Code)
            {
                yield return new ValidationResult("You cannot redirect to the Cofoundry admin user area because access rules only apply to custom user areas.", new string[] { nameof(UserAreaCodeForSignInRedirect) });
            }

            var duplicates = accessRules
                .GroupBy(r => new { r.UserAreaCode, r.RoleId })
                .Where(g => g.Count() > 1);

            foreach (var duplicate in duplicates)
            {
                var entity = duplicate.Key.RoleId.HasValue ? "role" : "user area";

                yield return new ValidationResult($"Duplicate {entity} detected in rule set.", new string[] { nameof(AccessRules) });
            }
        }
    }
}
