using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Mail;
using Cofoundry.Domain.MailTemplates;
using Microsoft.AspNetCore.Html;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Adds a user to the Cofoundry user area and sends a welcome notification.
    /// </summary>
    public class AddCofoundryUserCommandHandler
        : ICommandHandler<AddCofoundryUserCommand>
        , IPermissionRestrictedCommandHandler<AddCofoundryUserCommand>
    {
        #region constructor

        private readonly ICommandExecutor _commandExecutor;
        private readonly IPasswordCryptographyService _passwordCryptographyService;
        private readonly IPasswordGenerationService _passwordGenerationService;
        private readonly IMailService _mailService;
        private readonly IQueryExecutor _queryExecutor;

        public AddCofoundryUserCommandHandler(
            ICommandExecutor commandExecutor,
            IPasswordCryptographyService passwordCryptographyService,
            IPasswordGenerationService passwordGenerationService,
            IMailService mailService,
            IQueryExecutor queryExecutor
            )
        {
            _commandExecutor = commandExecutor;
            _passwordCryptographyService = passwordCryptographyService;
            _passwordGenerationService = passwordGenerationService;
            _mailService = mailService;
            _queryExecutor = queryExecutor;
        }

        #endregion

        #region execution

        public async Task ExecuteAsync(AddCofoundryUserCommand command, IExecutionContext executionContext)
        {
            var newUserCommand = MapCommand(command, executionContext);
            await _commandExecutor.ExecuteAsync(newUserCommand, executionContext);

            var siteSettingsQuery = new GetSettingsQuery<GeneralSiteSettings>();
            var siteSettings = await _queryExecutor.ExecuteAsync(siteSettingsQuery);
            var emailTemplate = MapEmailTemplate(newUserCommand, siteSettings);
            await _mailService.SendAsync(newUserCommand.Email, GetDisplayName(newUserCommand), emailTemplate);
        }

        #endregion

        #region private helpers

        private AddUserCommand MapCommand(AddCofoundryUserCommand command, IExecutionContext executionContext)
        {
            var newUserCommand = new AddUserCommand();
            newUserCommand.FirstName = command.FirstName;
            newUserCommand.LastName = command.LastName;
            newUserCommand.Email = command.Email;
            newUserCommand.Password = _passwordGenerationService.Generate();
            newUserCommand.RequirePasswordChange = true;
            newUserCommand.UserAreaCode = CofoundryAdminUserArea.AreaCode;
            newUserCommand.RoleId = command.RoleId;

            return newUserCommand;
        }

        private NewUserWelcomeMailTemplate MapEmailTemplate(AddUserCommand user, GeneralSiteSettings siteSettings)
        {
            var template = new NewUserWelcomeMailTemplate();
            template.FirstName = user.FirstName;
            template.LastName = user.LastName;
            template.TemporaryPassword = new HtmlString(user.Password);
            template.ApplicationName = siteSettings.ApplicationName;

            return template;
        }

        private string GetDisplayName(AddUserCommand command)
        {
            return (command.FirstName + " " + command.LastName).Trim();
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(AddCofoundryUserCommand command)
        {
            yield return new CofoundryUserCreatePermission();
        }

        #endregion
    }
}
