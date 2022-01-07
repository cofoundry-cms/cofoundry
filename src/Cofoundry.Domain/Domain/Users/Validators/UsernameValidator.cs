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
    public class UsernameValidator : IUsernameValidator
    {
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IDomainRepository _domainRepository;

        public UsernameValidator(
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IDomainRepository domainRepository
            )
        {
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _domainRepository = domainRepository;
        }

        public async virtual Task<ICollection<ValidationError>> GetErrorsAsync(IUsernameValidationContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var validators = new List<Func<IUsernameValidationContext, ValidationError>>()
            {
                ValidateUsernameFormattingResult,
                ValidateMinLength,
                ValidateMaxLength,
                ValidateAllowedCharacters,
            };

            var asyncValidators = new List<Func<IUsernameValidationContext, Task<ValidationError>>>()
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
        /// Validates that the <see cref="IUsernameValidationContext.Username"/>
        /// property is not <see langword="null"/>, which indicates that the formatting
        /// operation did not return a value. This should rarely happen, only if the username 
        /// contains only characters stripped out through a custom normalization process.
        /// </summary>
        protected virtual ValidationError ValidateUsernameFormattingResult(IUsernameValidationContext context)
        {
            if (context.Username != null) return null;

            return new ValidationError()
            {
                ErrorCode = FormatErrorCode("invalid-format"),
                Message = "Username is in an invalid format.",
                Properties = new string[] { context.PropertyName }
            };
        }


        /// <summary>
        /// Validates that the username is not shorter then the limit defined in the 
        /// <see cref="UsernameOptions"/> configuration settings.
        /// </summary>
        protected virtual ValidationError ValidateMinLength(IUsernameValidationContext context)
        {
            var options = _userAreaDefinitionRepository.GetOptionsByCode(context.UserAreaCode).Username;
            if (context.Username.NormalizedUsername.Length >= options.MinLength) return null;

            return new ValidationError()
            {
                ErrorCode = FormatErrorCode("min-length-not-met"),
                Message = $"Username cannot be less than {options.MinLength} characters.",
                Properties = new string[] { context.PropertyName }
            };
        }

        /// <summary>
        /// Validates that the username is not longer then the limit defined in the 
        /// <see cref="UsernameOptions"/> configuration settings.
        /// </summary>
        protected virtual ValidationError ValidateMaxLength(IUsernameValidationContext context)
        {
            var options = _userAreaDefinitionRepository.GetOptionsByCode(context.UserAreaCode).Username;
            if (context.Username.NormalizedUsername.Length <= options.MaxLength) return null;

            return new ValidationError()
            {
                ErrorCode = FormatErrorCode("max-length-exceeded"),
                Message = $"Username cannot be more than {options.MaxLength} characters.",
                Properties = new string[] { context.PropertyName }
            };
        }

        /// <summary>
        /// Validates that the username contains only the characters permitted by the 
        /// <see cref="UsernameOptions"/> configuration settings.
        /// </summary>
        protected virtual ValidationError ValidateAllowedCharacters(IUsernameValidationContext context)
        {
            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(context.UserAreaCode);
            
            // Bypass format validation for email-based usernames
            if (userArea.UseEmailAsUsername) return null;

            var options = _userAreaDefinitionRepository.GetOptionsByCode(context.UserAreaCode).Username;
            var invalidCharacters = UsernameCharacterValidator.GetInvalidCharacters(context.Username.NormalizedUsername, options);

            if (!invalidCharacters.Any()) return null;

            // Be careful here, because we're handling user input. Any message should be escaped when 
            // rendered, but to be safe we'll only include a single invalid character
            var msg = $"Username cannot contain '{invalidCharacters.First()}'.";

            return new ValidationError()
            {
                ErrorCode = FormatErrorCode("invalid-characters"),
                Message = msg,
                Properties = new string[] { context.PropertyName }
            };
        }

        /// <summary>
        /// Validates that the username is not already registered with another user.
        /// </summary>
        protected virtual async Task<ValidationError> ValidateUniqueAsync(IUsernameValidationContext context)
        {
            var query = new IsUsernameUniqueQuery()
            {
                Username = context.Username.UniqueUsername,
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
                Message = "This username is already registered.",
                Properties = new string[] { context.PropertyName }
            };
        }

        private ICollection<ValidationError> WrapError(ValidationError error)
        {
            return new List<ValidationError>() { error };
        }

        private static string FormatErrorCode(string errorCode)
        {
            return ValidationErrorCodes.AddNamespace(errorCode, "username");
        }
    }
}