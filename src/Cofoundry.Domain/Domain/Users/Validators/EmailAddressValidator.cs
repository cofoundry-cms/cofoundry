using Cofoundry.Core.Validation;
using Cofoundry.Core.Validation.Internal;
using Cofoundry.Domain.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Extendable
{
    /// <inheritdoc/>
    public class EmailAddressValidator : IEmailAddressValidator
    {
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IDomainRepository _domainRepository;

        public EmailAddressValidator(
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IDomainRepository domainRepository
            )
        {
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _domainRepository = domainRepository;
        }

        public virtual async Task<ICollection<ValidationError>> GetErrorsAsync(IEmailAddressValidationContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var validators = new List<Func<IEmailAddressValidationContext, ValidationError>>()
            {
                ValidateEmailFormattingResult,
                ValidateMinLength,
                ValidateMaxLength,
                ValidateAllowedCharacters,
            };

            var asyncValidators = new List<Func<IEmailAddressValidationContext, Task<ValidationError>>>()
            {
                ValidateUniqueAsync
            };

            foreach (var validator in validators)
            {
                var error = validator(context);
                if (error != null) return WrapError(error);
            }

            foreach (var asyncValidator in asyncValidators)
            {
                var error = await asyncValidator(context);
                if (error != null) return WrapError(error);
            }

            return new List<ValidationError>();
        }

        /// <summary>
        /// Validates that the <see cref="IEmailAddressValidationContext.Email"/>
        /// property is not <see langword="null"/>, which indicates that the formatting
        /// operation did not return a value. This can happen if the normalizer was supplied
        /// a value but could not parse it successfully.
        /// </summary>
        protected virtual ValidationError ValidateEmailFormattingResult(IEmailAddressValidationContext context)
        {
            if (context.Email != null) return null;

            return new ValidationError()
            {
                ErrorCode = FormatErrorCode("invalid-format"),
                Message = "Email is in an invalid format.",
                Properties = new string[] { context.PropertyName }
            };
        }

        /// <summary>
        /// Validates that the email is not shorter then the limit defined in the 
        /// <see cref="EmailAddressOptions"/> configuration settings.
        /// </summary>
        protected virtual ValidationError ValidateMinLength(IEmailAddressValidationContext context)
        {
            var options = _userAreaDefinitionRepository.GetOptionsByCode(context.UserAreaCode).EmailAddress;
            if (context.Email.NormalizedEmailAddress.Length >= options.MinLength) return null;

            return new ValidationError()
            {
                ErrorCode = FormatErrorCode("min-length-not-met"),
                Message = $"Email cannot be less than {options.MinLength} characters.",
                Properties = new string[] { context.PropertyName }
            };
        }

        /// <summary>
        /// Validates that the email is not longer then the limit defined in the 
        /// <see cref="EmailAddressOptions"/> configuration settings.
        /// </summary>
        protected virtual ValidationError ValidateMaxLength(IEmailAddressValidationContext context)
        {
            var options = _userAreaDefinitionRepository.GetOptionsByCode(context.UserAreaCode).EmailAddress;
            if (context.Email.NormalizedEmailAddress.Length <= options.MaxLength) return null;

            return new ValidationError()
            {
                ErrorCode = FormatErrorCode("max-length-exceeded"),
                Message = $"Email cannot be more than {options.MaxLength} characters.",
                Properties = new string[] { context.PropertyName }
            };
        }

        /// <summary>
        /// Validates that the email contains only the characters permitted by the 
        /// <see cref="EmailAddressOptions"/> configuration settings.
        /// </summary>
        protected virtual ValidationError ValidateAllowedCharacters(IEmailAddressValidationContext context)
        {
            var options = _userAreaDefinitionRepository.GetOptionsByCode(context.UserAreaCode).EmailAddress;
            var invalidCharacters = EmailAddressCharacterValidator.GetInvalidCharacters(context.Email.NormalizedEmailAddress, options);

            if (!invalidCharacters.Any()) return null;

            // Be careful here, because we're handling user input. Any message should be escaped when 
            // rendered, but to be safe we'll only include a single invalid character
            var msg = $"Email cannot contain '{invalidCharacters.First()}'.";

            return new ValidationError()
            {
                ErrorCode = FormatErrorCode("invalid-characters"),
                Message = msg,
                Properties = new string[] { context.PropertyName }
            };
        }

        /// <summary>
        /// Validates that the email is not already registered with another user.
        /// </summary>
        protected virtual async Task<ValidationError> ValidateUniqueAsync(IEmailAddressValidationContext context)
        {
            var query = new IsUserEmailAddressUniqueQuery()
            {
                Email = context.Email.UniqueEmailAddress,
                UserAreaCode = context.UserAreaCode
            };

            if (context.UserId.HasValue)
            {
                query.UserId = context.UserId.Value;
            }

            var isUnique = await _domainRepository
                .WithExecutionContext(context.ExecutionContext)
                .ExecuteQueryAsync(query);

            if (isUnique) return null;

            return new ValidationError()
            {
                ErrorCode = FormatErrorCode("not-unique"),
                Message = "This email is already registered.",
                Properties = new string[] { context.PropertyName }
            };
        }

        private ICollection<ValidationError> WrapError(ValidationError error)
        {
            return new List<ValidationError>() { error };
        }

        private static string FormatErrorCode(string errorCode)
        {
            return ValidationErrorCodes.AddNamespace(errorCode, "user-email");
        }
    }
}