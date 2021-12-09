using Cofoundry.Core.Validation;
using Cofoundry.Domain.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <inheritdoc/>
    public class PasswordPolicy : IPasswordPolicy
    {
        private readonly ICollection<INewPasswordValidatorBase> _validators;

        public PasswordPolicy(
            string description,
            ICollection<INewPasswordValidatorBase> validators,
            IDictionary<string, string> attributes
            )
        {
            Description = description;
            _validators = validators;
            Attributes = attributes;
        }

        public string Description { get; }

        public IDictionary<string, string> Attributes { get; }

        public IEnumerable<string> GetCriteria()
        {
            return _validators
                .Where(v => v.Criteria != null)
                .Select(v => v.Criteria);
        }

        public async Task<ICollection<ValidationError>> ValidateAsync(
            INewPasswordValidationContext newPasswordValidatonContext
            )
        {
            var errors = new List<ValidationError>(_validators.Count);

            foreach (var validator in _validators.OrderBy(v => v is IAsyncNewPasswordValidator))
            {
                ValidationError error = null;

                if (validator is INewPasswordValidator syncValidator)
                {
                    error = syncValidator.Validate(newPasswordValidatonContext);
                }
                else if (validator is IAsyncNewPasswordValidator asyncValidator)
                {
                    // Only aggregate sync errors because async errors are likely slow
                    if (errors.Any()) return errors;

                    error = await asyncValidator.ValidateAsync(newPasswordValidatonContext);
                }
                else
                {
                    throw new InvalidOperationException($"Policy contains an unrecognised {nameof(INewPasswordValidatorBase)} implementation: {validator.GetType().Name}");
                }

                if (error != null)
                {
                    errors.Add(error);
                }
            }

            return errors;
        }
    }
}
