using Cofoundry.Core.Mail;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Internal;
using Cofoundry.Domain.MailTemplates;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <inheritdoc/>
    public class PasswordUpdateCommandHelper : IPasswordUpdateCommandHelper
    {
        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IPasswordCryptographyService _passwordCryptographyService;
        private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
        private readonly IUserSummaryMapper _userSummaryMapper;
        private readonly IMailService _mailService;

        public PasswordUpdateCommandHelper(
            IQueryExecutor queryExecutor,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IPermissionValidationService permissionValidationService,
            IPasswordCryptographyService passwordCryptographyService,
            IMailService mailService,
            IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
            IUserSummaryMapper userSummaryMapper
            )
        {
            _queryExecutor = queryExecutor;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _passwordCryptographyService = passwordCryptographyService;
            _permissionValidationService = permissionValidationService;
            _mailService = mailService;
            _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
            _userSummaryMapper = userSummaryMapper;
        }

        public void ValidateUserArea(IUserAreaDefinition userArea)
        {
            if (!userArea.AllowPasswordLogin)
            {
                throw new InvalidOperationException("Cannot update the password to account in a user area that does not allow password logins.");
            }
        }

        public void ValidatePermissions(IUserAreaDefinition userArea, IExecutionContext executionContext)
        {
            if (userArea is CofoundryAdminUserArea)
            {
                _permissionValidationService.EnforcePermission(new CofoundryUserUpdatePermission(), executionContext.UserContext);
            }
            else
            {
                _permissionValidationService.EnforcePermission(new NonCofoundryUserUpdatePermission(), executionContext.UserContext);
            }
        }

        public void UpdatePassword(string newPassword, User user, IExecutionContext executionContext)
        {
            user.RequirePasswordChange = false;
            user.LastPasswordChangeDate = executionContext.ExecutionDate;

            UpdatePasswordHash(newPassword, user);
        }

        public void UpdatePasswordHash(string newPassword, User user)
        {
            var hashResult = _passwordCryptographyService.CreateHash(newPassword);
            user.Password = hashResult.Hash;
            user.PasswordHashVersion = hashResult.HashVersion;
        }

        public async Task SendPasswordChangedNotification(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var options = _userAreaDefinitionRepository.GetOptionsByCode(user.UserAreaCode).Password;
            if (!options.SendNotificationOnUpdate) return;

            var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(user.UserAreaCode);

            var context = new PasswordChangedTemplateBuilderContext();
            context.User = _userSummaryMapper.Map(user);
            var mailTemplate = await mailTemplateBuilder.BuildPasswordChangedTemplateAsync(context);

            // Null template means don't send a notification
            if (mailTemplate == null) return;

            await _mailService.SendAsync(user.Email, mailTemplate);
        }
    }
}