using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class UserUpdateCommandHelper : IUserUpdateCommandHelper
    {
        private const string EMAIL_PROPERTY = "Email";
        private const string USERNAME_PROPERTY = "Username";

        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IUserDataFormatter _userDataFormatter;
        private readonly IUserStoredProcedures _userStoredProcedures;
        private readonly IQueryExecutor _queryExecutor;

        public UserUpdateCommandHelper(
            IUserAreaDefinitionRepository userAreaRepository,
            IUserDataFormatter userDataFormatter,
            IUserStoredProcedures userStoredProcedures,
            IQueryExecutor queryExecutor
            )
        {
            _userAreaRepository = userAreaRepository;
            _userDataFormatter = userDataFormatter;
            _userStoredProcedures = userStoredProcedures;
            _queryExecutor = queryExecutor;
        }

        public async Task UpdateEmailAndUsernameAsync(
            string email, 
            string username, 
            User user, 
            IExecutionContext executionContext
            )
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));

            var userAreaCode = user.UserArea?.UserAreaCode ?? user.UserAreaCode;
            if (userAreaCode == null)
            {
                throw new ArgumentException("user parameter must have a user area set.", nameof(user));
            }
            var userArea = _userAreaRepository.GetRequiredByCode(userAreaCode);
            
            await UpdateEmailAsync(userArea, email, username, user, executionContext);

            if (!userArea.UseEmailAsUsername)
            {
                if (username == null)
                {
                    throw ValidationErrorException.CreateWithProperties("Username field is required", USERNAME_PROPERTY);
                }

                var usernameFormatResult = _userDataFormatter.FormatUsername(userArea, username);
                await UpdateUsernameAsync(userArea, usernameFormatResult, user, executionContext);
            }
        }

        public async Task UpdateEmailAsync(
            IUserAreaDefinition userArea,
            string email,
            string username,
            User user,
            IExecutionContext executionContext
            )
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (executionContext == null) throw new ArgumentNullException(nameof(executionContext));

            if (userArea.UseEmailAsUsername && !string.IsNullOrEmpty(username))
            {
                throw ValidationErrorException.CreateWithProperties("Username field should be empty becuase the specified user area uses the email as the username.", USERNAME_PROPERTY);
            }

            var isEmailDefined = !string.IsNullOrWhiteSpace(email);
            if (!isEmailDefined && userArea.UseEmailAsUsername)
            {
                throw ValidationErrorException.CreateWithProperties("Email field is required.", EMAIL_PROPERTY);
            }
            else if (!isEmailDefined)
            {
                user.Email = null;
                user.UniqueEmail = null;
                user.EmailDomainId = null;

                return;
            }

            var emailFormatResult = _userDataFormatter.FormatEmailAddress(userArea, email);
            if (emailFormatResult == null)
            {
                throw ValidationErrorException.CreateWithProperties("Email is in an invalid format.", EMAIL_PROPERTY);
            }

            var hasEmailChanged = user.Email != emailFormatResult.NormalizedEmailAddress || user.UniqueEmail != emailFormatResult.UniqueEmailAddress;
            if (!hasEmailChanged) return;

            await ValidateNewEmailAsync(userArea, emailFormatResult, user, executionContext);

            user.EmailDomainId = await GetEmailDomainIdAsync(emailFormatResult, executionContext);
            user.Email = emailFormatResult.NormalizedEmailAddress;
            user.UniqueEmail = emailFormatResult.UniqueEmailAddress;

            if (userArea.UseEmailAsUsername)
            {
                var usernameFormatterResult = _userDataFormatter.FormatUsername(userArea, emailFormatResult);
                if (usernameFormatterResult == null)
                {
                    // This shouldn't happen unless a custom email uniquifier is misbehaving
                    throw ValidationErrorException.CreateWithProperties("Email is invalid as a username.", EMAIL_PROPERTY);
                }

                await UpdateUsernameAsync(userArea, usernameFormatterResult, user, executionContext);
            }
        }

        private async Task UpdateUsernameAsync(
            IUserAreaDefinition userArea,
            UsernameFormattingResult username,
            User user,
            IExecutionContext executionContext
            )
        {
            if (user.Username == username.NormalizedUsername 
                && user.UniqueUsername == username.UniqueUsername)
            {
                return;
            }

            await ValidateNewUsernameAsync(userArea, username, user, executionContext);

            user.Username = username.NormalizedUsername;
            user.UniqueUsername = username.UniqueUsername;
        }

        private async Task ValidateNewEmailAsync(
            IUserAreaDefinition userArea,
            EmailAddressFormattingResult emailAddress,
            User user,
            IExecutionContext executionContext
            )
        {
            var query = new IsEmailUniqueQuery()
            {
                Email = emailAddress.UniqueEmailAddress,
                UserAreaCode = userArea.UserAreaCode
            };

            if (user.UserId > 0)
            {
                query.UserId = user.UserId;
            }

            if (!await _queryExecutor.ExecuteAsync(query, executionContext))
            {
                throw ValidationErrorException.CreateWithProperties("This email is already registered", EMAIL_PROPERTY);
            }
        }

        private async Task ValidateNewUsernameAsync(
            IUserAreaDefinition userArea,
            UsernameFormattingResult username,
            User user,
            IExecutionContext executionContext
            )
        {
            var errorProperty = userArea.UseEmailAsUsername ? EMAIL_PROPERTY : USERNAME_PROPERTY;

            if (username == null)
            {
                // This should rarely happen, only if the username contains only characters 
                // stripped out through a custom normalization process
                throw ValidationErrorException.CreateWithProperties("Username is in an invalid format.", errorProperty);
            }

            var query = new IsUsernameUniqueQuery()
            {
                Username = username.UniqueUsername,
                UserAreaCode = userArea.UserAreaCode
            };

            if (user.UserId > 0)
            {
                query.UserId = user.UserId;
            }

            if (!await _queryExecutor.ExecuteAsync(query, executionContext))
            {
                throw ValidationErrorException.CreateWithProperties("This username is already registered", errorProperty);
            }
        }

        private async Task<int?> GetEmailDomainIdAsync(EmailAddressFormattingResult emailAddress, IExecutionContext executionContext)
        {
            if (emailAddress == null) return null;

            var emailDomainId = await _userStoredProcedures.AddEmailDomainIfNotExists(
                emailAddress.Domain.Name,
                emailAddress.Domain.IdnName,
                executionContext.ExecutionDate
                );

            return emailDomainId;
        }
    }
}
