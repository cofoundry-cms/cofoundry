using Cofoundry.Core.MessageAggregator;
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
        private readonly IMessageAggregator _messageAggregator;
        private readonly IEmailAddressValidator _emailAddressValidator;
        private readonly IUsernameValidator _usernameValidator;

        public UserUpdateCommandHelper(
            IUserAreaDefinitionRepository userAreaRepository,
            IUserDataFormatter userDataFormatter,
            IUserStoredProcedures userStoredProcedures,
            IMessageAggregator messageAggregator,
            IEmailAddressValidator emailAddressValidator,
            IUsernameValidator usernameValidator
            )
        {
            _userAreaRepository = userAreaRepository;
            _userDataFormatter = userDataFormatter;
            _userStoredProcedures = userStoredProcedures;
            _messageAggregator = messageAggregator;
            _emailAddressValidator = emailAddressValidator;
            _usernameValidator = usernameValidator;
        }

        public async Task<UpdateEmailAndUsernameResult> UpdateEmailAndUsernameAsync(
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

            var result = await UpdateEmailAsync(userArea, email, username, user, executionContext);

            if (!userArea.UseEmailAsUsername)
            {
                if (username == null)
                {
                    throw ValidationErrorException.CreateWithProperties("Username field is required", USERNAME_PROPERTY);
                }

                var usernameFormatResult = _userDataFormatter.FormatUsername(userArea, username);
                result.HasUsernameChanged = await UpdateUsernameAsync(userArea, usernameFormatResult, user, executionContext);
            }

            return result;
        }

        public async Task PublishUpdateMessagesAsync(User user, UpdateEmailAndUsernameResult updateResult)
        {
            await _messageAggregator.PublishAsync(new UserUpdatedMessage()
            {
                UserAreaCode = user.UserAreaCode,
                UserId = user.UserId
            });

            if (updateResult.HasEmailChanged)
            {
                await _messageAggregator.PublishAsync(new UserEmailUpdatedMessage()
                {
                    UserAreaCode = user.UserAreaCode,
                    UserId = user.UserId
                });
            }

            if (updateResult.HasUsernameChanged)
            {
                await _messageAggregator.PublishAsync(new UserUsernameUpdatedMessage()
                {
                    UserAreaCode = user.UserAreaCode,
                    UserId = user.UserId
                });
            }
        }

        private async Task<UpdateEmailAndUsernameResult> UpdateEmailAsync(
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

            var result = new UpdateEmailAndUsernameResult();
            var isEmailDefined = !string.IsNullOrWhiteSpace(email);

            if (!isEmailDefined && (userArea.UseEmailAsUsername || userArea.AllowPasswordLogin))
            {
                throw ValidationErrorException.CreateWithProperties("Email field is required.", EMAIL_PROPERTY);
            }
            else if (!isEmailDefined && !string.IsNullOrWhiteSpace(user.Email))
            {
                user.Email = null;
                user.UniqueEmail = null;
                user.EmailDomainId = null;
                user.IsEmailConfirmed = false;

                result.HasEmailChanged = true;
                return result;
            }
            else if (!isEmailDefined)
            {
                return result;
            }

            var emailFormatResult = _userDataFormatter.FormatEmailAddress(userArea, email);
            if (emailFormatResult == null)
            {
                throw ValidationErrorException.CreateWithProperties("Email is in an invalid format.", EMAIL_PROPERTY);
            }

            result.HasEmailChanged = user.Email != emailFormatResult.NormalizedEmailAddress || user.UniqueEmail != emailFormatResult.UniqueEmailAddress;
            if (!result.HasEmailChanged) return result;

            await ValidateNewEmailAsync(userArea, emailFormatResult, user, executionContext);

            user.EmailDomainId = await GetEmailDomainIdAsync(emailFormatResult, executionContext);
            user.Email = emailFormatResult.NormalizedEmailAddress;
            user.UniqueEmail = emailFormatResult.UniqueEmailAddress;
            user.IsEmailConfirmed = false;

            if (userArea.UseEmailAsUsername)
            {
                var usernameFormatterResult = _userDataFormatter.FormatUsername(userArea, emailFormatResult);
                if (usernameFormatterResult == null)
                {
                    // This shouldn't happen unless a custom email uniquifier is misbehaving
                    throw ValidationErrorException.CreateWithProperties("Email is invalid as a username.", EMAIL_PROPERTY);
                }

                result.HasUsernameChanged = await UpdateUsernameAsync(userArea, usernameFormatterResult, user, executionContext);
            }

            return result;
        }

        private async Task<bool> UpdateUsernameAsync(
            IUserAreaDefinition userArea,
            UsernameFormattingResult username,
            User user,
            IExecutionContext executionContext
            )
        {
            if (user.Username == username.NormalizedUsername 
                && user.UniqueUsername == username.UniqueUsername)
            {
                return false;
            }

            await ValidateNewUsernameAsync(userArea, username, user, executionContext);

            user.Username = username.NormalizedUsername;
            user.UniqueUsername = username.UniqueUsername;

            return true;
        }

        private async Task ValidateNewEmailAsync(
            IUserAreaDefinition userArea,
            EmailAddressFormattingResult emailAddress,
            User user,
            IExecutionContext executionContext
            )
        {
            var context = new EmailAddressValidationContext()
            {
                Email = emailAddress,
                ExecutionContext = executionContext,
                PropertyName = EMAIL_PROPERTY,
                UserAreaCode = userArea.UserAreaCode
            };

            if (user.UserId > 0)
            {
                context.UserId = user.UserId;
            }

            await _emailAddressValidator.ValidateAsync(context);
        }

        private async Task ValidateNewUsernameAsync(
            IUserAreaDefinition userArea,
            UsernameFormattingResult username,
            User user,
            IExecutionContext executionContext
            )
        {
            var context = new UsernameValidationContext()
            {
                Username = username,
                ExecutionContext = executionContext,
                PropertyName = userArea.UseEmailAsUsername ? EMAIL_PROPERTY : USERNAME_PROPERTY,
                UserAreaCode = userArea.UserAreaCode
            };

            if (user.UserId > 0)
            {
                context.UserId = user.UserId;
            }

            await _usernameValidator.ValidateAsync(context);
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

        public class UpdateEmailAndUsernameResult
        {
            public bool HasEmailChanged { get; set; }

            public bool HasUsernameChanged { get; set; }
        }
    }
}
