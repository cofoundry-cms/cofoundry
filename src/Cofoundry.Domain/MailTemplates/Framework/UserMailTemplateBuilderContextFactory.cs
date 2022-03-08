using Cofoundry.Core.Web;
using Microsoft.AspNetCore.Html;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.MailTemplates.Internal
{
    public class UserMailTemplateBuilderContextFactory : IUserMailTemplateBuilderContextFactory
    {
        private readonly ISiteUrlResolver _siteUrlResolver;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IAuthorizedTaskTokenUrlHelper _authorizedTaskTokenUrlHelper;
        private readonly IUserMailTemplateInitializer _userMailTemplateInitializer;

        public UserMailTemplateBuilderContextFactory(
            ISiteUrlResolver siteUrlResolver,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IAuthorizedTaskTokenUrlHelper authorizedTaskTokenUrlHelper,
            IUserMailTemplateInitializer userMailTemplateInitializer
            )
        {
            _siteUrlResolver = siteUrlResolver;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _authorizedTaskTokenUrlHelper = authorizedTaskTokenUrlHelper;
            _userMailTemplateInitializer = userMailTemplateInitializer;
        }

        public IAccountRecoveryTemplateBuilderContext CreateAccountRecoveryContext(UserSummary user, string token)
        {
            var definition = _userAreaDefinitionRepository.GetRequiredByCode(user.UserArea.UserAreaCode);
            var options = _userAreaDefinitionRepository.GetOptionsByCode(user.UserArea.UserAreaCode).AccountRecovery;
            var isCofoundryAdmin = definition is CofoundryAdminUserArea;

            var context = new AccountRecoveryTemplateBuilderContext()
            {
                User = user,
                Token = token,
                DefaultTemplateFactory = AccountRecoveryTemplateFactory
            };

            // Can be null to allow a custom IDefaultMailTemplateBuilder implementation to the path explicitly
            if (options.RecoveryUrlBase != null)
            {
                context.RecoveryUrlPath = _authorizedTaskTokenUrlHelper.MakeUrl(options.RecoveryUrlBase, token);
            }

            return context;
        }

        private async Task<AccountRecoveryMailTemplate> AccountRecoveryTemplateFactory(AccountRecoveryTemplateBuilderContext context)
        {

            if (context.RecoveryUrlPath == null || !Uri.IsWellFormedUriString(context.RecoveryUrlPath, UriKind.Relative))
            {
                // The RecoveryUrlPath setting isn't required in config because the feature may not be used, or may 
                // be manually constructed in a custom IDefaultMailTemplateBuilder implementation. However it should be 
                // supplied at this point for our default builder.
                var options = _userAreaDefinitionRepository.GetOptionsByCode(context.User.UserArea.UserAreaCode).AccountRecovery;
                if (string.IsNullOrEmpty(options.RecoveryUrlBase))
                {
                    throw new InvalidOperationException($"To use the account recovery feature you must configure the {nameof(AccountRecoveryOptions.RecoveryUrlBase)} setting by implementing {nameof(IUserAreaDefinition)}.{nameof(IUserAreaDefinition.ConfigureOptions)}.");
                }

                throw new InvalidOperationException($"{nameof(AccountRecoveryTemplateBuilderContext)}.{nameof(context.RecoveryUrlPath)} should be a valid relative uri.");
            }

            var template = new AccountRecoveryMailTemplate();
            template.RecoveryUrl = _siteUrlResolver.MakeAbsolute(context.RecoveryUrlPath);
            await _userMailTemplateInitializer.Initialize(context.User, template);

            return template;
        }

        public IAccountVerificationTemplateBuilderContext CreateAccountVerificationContext(UserSummary user, string token)
        {
            var context = new AccountVerificationTemplateBuilderContext()
            {
                User = user,
                Token = token,
                DefaultTemplateFactory = AccountVerificationTemplateFactory
            };

            // Can be null to allow a custom IDefaultMailTemplateBuilder implementation to the path explicitly
            var options = _userAreaDefinitionRepository.GetOptionsByCode(user.UserArea.UserAreaCode).AccountVerification;
            if (options.VerificationUrlBase != null)
            {
                context.VerificationUrlPath = _authorizedTaskTokenUrlHelper.MakeUrl(options.VerificationUrlBase, token);
            }

            return context;
        }

        private async Task<AccountVerificationMailTemplate> AccountVerificationTemplateFactory(AccountVerificationTemplateBuilderContext context)
        {
            var userAreaDefinition = _userAreaDefinitionRepository.GetRequiredByCode(context.User.UserArea.UserAreaCode);
            if (userAreaDefinition is CofoundryAdminUserArea)
            {
                // Admin user area does not implement account verification
                throw new NotImplementedException();
            }

            if (context.VerificationUrlPath == null || !Uri.IsWellFormedUriString(context.VerificationUrlPath, UriKind.Relative))
            {
                // The VerificationUrlPath setting isn't required in config because the feature may not be used, or may 
                // be manually constructed in a custom IDefaultMailTemplateBuilder implementation. However it should be 
                // supplied at this point for our default builder.
                var options = _userAreaDefinitionRepository.GetOptionsByCode(userAreaDefinition.UserAreaCode).AccountVerification;
                if (string.IsNullOrEmpty(options.VerificationUrlBase))
                {
                    throw new InvalidOperationException($"To use the account verification feature you must configure the {nameof(AccountRecoveryOptions.RecoveryUrlBase)} setting by implementing {nameof(IUserAreaDefinition)}.{nameof(IUserAreaDefinition.ConfigureOptions)}.");
                }

                throw new InvalidOperationException($"{nameof(AccountRecoveryTemplateBuilderContext)}.{nameof(context.VerificationUrlPath)} should be a valid relative uri.");
            }

            var template = new AccountVerificationMailTemplate();
            template.VerificationUrl = _siteUrlResolver.MakeAbsolute(context.VerificationUrlPath);
            await _userMailTemplateInitializer.Initialize(context.User, template);

            return template;
        }

        public INewUserWithTemporaryPasswordTemplateBuilderContext CreateNewUserWithTemporaryPasswordContext(UserSummary user, string temporaryPassword)
        {
            var context = new NewUserWithTemporaryPasswordTemplateBuilderContext()
            {
                User = user,
                TemporaryPassword = new HtmlString(temporaryPassword),
                DefaultTemplateFactory = NewUserWithTemporaryPasswordTemplateFactory
            };

            return context;
        }

        private async Task<NewUserWithTemporaryPasswordMailTemplate> NewUserWithTemporaryPasswordTemplateFactory(NewUserWithTemporaryPasswordTemplateBuilderContext context)
        {
            var template = new NewUserWithTemporaryPasswordMailTemplate();
            template.TemporaryPassword = context.TemporaryPassword;
            await _userMailTemplateInitializer.Initialize(context.User, template);

            return template;
        }

        public IPasswordChangedTemplateBuilderContext CreatePasswordChangedContext(UserSummary user)
        {
            var context = new PasswordChangedTemplateBuilderContext()
            {
                User = user,
                DefaultTemplateFactory = PasswordChangedTemplateFactory
            };

            return context;
        }

        private async Task<PasswordChangedMailTemplate> PasswordChangedTemplateFactory(PasswordChangedTemplateBuilderContext context)
        {
            var template = new PasswordChangedMailTemplate();
            await _userMailTemplateInitializer.Initialize(context.User, template);

            return template;
        }

        public IPasswordResetTemplateBuilderContext CreatePasswordResetContext(UserSummary user, string temporaryPassword)
        {
            var context = new PasswordResetTemplateBuilderContext()
            {
                User = user,
                TemporaryPassword = new HtmlString(temporaryPassword),
                DefaultTemplateFactory = PasswordResetTemplateFactory
            };

            return context;
        }

        private async Task<PasswordResetMailTemplate> PasswordResetTemplateFactory(PasswordResetTemplateBuilderContext context)
        {
            var template = new PasswordResetMailTemplate();
            template.TemporaryPassword = context.TemporaryPassword;
            await _userMailTemplateInitializer.Initialize(context.User, template);

            return template;
        }
    }
}
