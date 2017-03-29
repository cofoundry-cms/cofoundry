using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Mail;
using Cofoundry.Domain.MailTemplates;
using Microsoft.AspNetCore.Html;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Adds a user to the Cofoundry user area and sends a welcome notification.
    /// </summary>
    /// <remarks>
    public class AddCofoundryUserCommandHandler
        : ICommandHandler<AddCofoundryUserCommand>
        , IAsyncCommandHandler<AddCofoundryUserCommand>
        , IPermissionRestrictedCommandHandler<AddCofoundryUserCommand>
    {
        #region constructor

        private readonly ICommandExecutor _commandExecutor;
        private readonly IPasswordCryptographyService _passwordCryptographyService;
        private readonly IPasswordGenerationService _passwordGenerationService;
        private readonly IMailService _mailService;

        public AddCofoundryUserCommandHandler(
            ICommandExecutor commandExecutor,
            IPasswordCryptographyService passwordCryptographyService,
            IPasswordGenerationService passwordGenerationService,
            IMailService mailService
            )
        {
            _commandExecutor = commandExecutor;
            _passwordCryptographyService = passwordCryptographyService;
            _passwordGenerationService = passwordGenerationService;
            _mailService = mailService;
        }

        #endregion

        #region execution

        public void Execute(AddCofoundryUserCommand command, IExecutionContext executionContext)
        {
            var newUserCommand = MapCommand(command, executionContext);
            _commandExecutor.Execute(newUserCommand, executionContext);

            var emailTemplate = MapEmailTemplate(newUserCommand);
            _mailService.Send(newUserCommand.Email, GetDisplayName(newUserCommand), emailTemplate);
        }

        public async Task ExecuteAsync(AddCofoundryUserCommand command, IExecutionContext executionContext)
        {
            var newUserCommand = MapCommand(command, executionContext);
            await _commandExecutor.ExecuteAsync(newUserCommand, executionContext);

            var emailTemplate = MapEmailTemplate(newUserCommand);
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

        private NewUserWelcomeMailTemplate MapEmailTemplate(AddUserCommand user)
        {
            var template = new NewUserWelcomeMailTemplate();
            template.FirstName = user.FirstName;
            template.LastName = user.LastName;
            template.TemporaryPassword = new HtmlString(user.Password);

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
