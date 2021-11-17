using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Mail;
using Cofoundry.Domain.MailTemplates;
using Microsoft.AspNetCore.Html;
using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Adds a user to the Cofoundry user area and sends a welcome notification.
    /// </summary>
    public class AddUserWithTemporaryPasswordCommandHandler
        : ICommandHandler<AddUserWithTemporaryPasswordCommand>
        , IPermissionRestrictedCommandHandler<AddUserWithTemporaryPasswordCommand>
    {
        #region constructor

        private readonly ICommandExecutor _commandExecutor;
        private readonly IPasswordGenerationService _passwordGenerationService;
        private readonly IMailService _mailService;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly CofoundryDbContext _dbContext;

        public AddUserWithTemporaryPasswordCommandHandler(
            ICommandExecutor commandExecutor,
            IPasswordGenerationService passwordGenerationService,
            IMailService mailService,
            IQueryExecutor queryExecutor,
            IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            ITransactionScopeManager transactionScopeFactory,
            CofoundryDbContext dbContext
            )
        {
            _commandExecutor = commandExecutor;
            _passwordGenerationService = passwordGenerationService;
            _mailService = mailService;
            _queryExecutor = queryExecutor;
            _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _transactionScopeFactory = transactionScopeFactory;
            _dbContext = dbContext;
        }

        #endregion

        public async Task ExecuteAsync(AddUserWithTemporaryPasswordCommand command, IExecutionContext executionContext)
        {
            ValidateUserArea(command);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                var newUserCommand = MapCommand(command, executionContext);
                await _commandExecutor.ExecuteAsync(newUserCommand, executionContext);

                await SendNotificationAsync(newUserCommand);

                await scope.CompleteAsync();
            }
        }

        private async Task SendNotificationAsync(AddUserCommand newUserCommand)
        {
            // Send mail notification
            var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(newUserCommand.UserAreaCode);

            var context = await CreateMailTemplateContextAsync(newUserCommand);
            var mailTemplate = await mailTemplateBuilder.BuildNewUserWithTemporaryPasswordTemplateAsync(context);

            // Null template means don't send a notification
            if (mailTemplate == null) return;

            await _mailService.SendAsync(newUserCommand.Email, mailTemplate);
        }

        private void ValidateUserArea(AddUserWithTemporaryPasswordCommand command)
        {
            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(command.UserAreaCode);
            EntityNotFoundException.ThrowIfNull(userArea, command.UserAreaCode);
            if (!userArea.AllowPasswordLogin)
            {
                throw new InvalidOperationException(nameof(AddUserWithTemporaryPasswordCommand) + " must be used with a user area that supports password based logins.");
            }

            if (!userArea.UseEmailAsUsername)
            {
                throw new InvalidOperationException(nameof(AddUserWithTemporaryPasswordCommand) + " must be used with a user area that supports email logins.");
            }
        }

        private AddUserCommand MapCommand(AddUserWithTemporaryPasswordCommand command, IExecutionContext executionContext)
        {
            var newUserCommand = new AddUserCommand()
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                Password = _passwordGenerationService.Generate(),
                RequirePasswordChange = true,
                UserAreaCode = command.UserAreaCode,
                RoleId = command.RoleId
            };

            return newUserCommand;
        }

        private async Task<NewUserWithTemporaryPasswordTemplateBuilderContext> CreateMailTemplateContextAsync(AddUserCommand newUserCommand)
        {
            var query = new GetUserSummaryByIdQuery(newUserCommand.OutputUserId);
            var user = await _queryExecutor.ExecuteAsync(query);
            EntityNotFoundException.ThrowIfNull(user, newUserCommand.OutputUserId);

            var context = new NewUserWithTemporaryPasswordTemplateBuilderContext()
            {
                User = user,
                TemporaryPassword = new HtmlString(newUserCommand.Password)
            };

            return context;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(AddUserWithTemporaryPasswordCommand command)
        {
            if (command.UserAreaCode == CofoundryAdminUserArea.AreaCode)
            {
                yield return new CofoundryUserCreatePermission();
            }
            else
            {
                yield return new NonCofoundryUserCreatePermission();
            }
        }

        #endregion
    }
}
