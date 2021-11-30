using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Core.Mail;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.MailTemplates;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Adds a new user and sends a notification containing a generated 
    /// password which must be changed at first login.
    /// </summary>
    public class AddUserWithTemporaryPasswordCommandHandler
        : ICommandHandler<AddUserWithTemporaryPasswordCommand>
        , IPermissionRestrictedCommandHandler<AddUserWithTemporaryPasswordCommand>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IPasswordGenerationService _passwordGenerationService;
        private readonly IMailService _mailService;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly ITransactionScopeManager _transactionScopeFactory;
        private readonly IEmailAddressNormalizer _emailAddressNormalizer;

        public AddUserWithTemporaryPasswordCommandHandler(
            CofoundryDbContext dbContext,
            ICommandExecutor commandExecutor,
            IPasswordGenerationService passwordGenerationService,
            IMailService mailService,
            IQueryExecutor queryExecutor,
            IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            ITransactionScopeManager transactionScopeFactory,
            IEmailAddressNormalizer emailAddressNormalizer
            )
        {
            _dbContext = dbContext;
            _commandExecutor = commandExecutor;
            _passwordGenerationService = passwordGenerationService;
            _mailService = mailService;
            _queryExecutor = queryExecutor;
            _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _transactionScopeFactory = transactionScopeFactory;
            _emailAddressNormalizer = emailAddressNormalizer;
        }

        public async Task ExecuteAsync(AddUserWithTemporaryPasswordCommand command, IExecutionContext executionContext)
        {
            ValidateUserArea(command);
            Normalize(command);

            using (var scope = _transactionScopeFactory.Create(_dbContext))
            {
                var newUserCommand = MapCommand(command);
                await _commandExecutor.ExecuteAsync(newUserCommand, executionContext);
                await SendNotificationAsync(newUserCommand, executionContext);
                command.OutputUserId = newUserCommand.OutputUserId;

                await scope.CompleteAsync();
            }
        }

        private void Normalize(AddUserWithTemporaryPasswordCommand command)
        {
            command.Email = _emailAddressNormalizer.Normalize(command.Email);
        }

        private async Task SendNotificationAsync(AddUserCommand newUserCommand, IExecutionContext executionContext)
        {
            // Send mail notification
            var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(newUserCommand.UserAreaCode);

            var context = await CreateMailTemplateContextAsync(newUserCommand, executionContext);
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

        private AddUserCommand MapCommand(AddUserWithTemporaryPasswordCommand command)
        {
            var newUserCommand = new AddUserCommand()
            {
                FirstName = command.FirstName,
                LastName = command.LastName,
                Email = command.Email,
                Password = _passwordGenerationService.Generate(),
                RequirePasswordChange = true,
                UserAreaCode = command.UserAreaCode,
                RoleId = command.RoleId,
                RoleCode = command.RoleCode
            };

            return newUserCommand;
        }

        private async Task<NewUserWithTemporaryPasswordTemplateBuilderContext> CreateMailTemplateContextAsync(AddUserCommand newUserCommand, IExecutionContext executionContext)
        {
            var query = new GetUserSummaryByIdQuery(newUserCommand.OutputUserId);
            var user = await _queryExecutor.ExecuteAsync(query, executionContext);
            EntityNotFoundException.ThrowIfNull(user, newUserCommand.OutputUserId);

            var context = new NewUserWithTemporaryPasswordTemplateBuilderContext()
            {
                User = user,
                TemporaryPassword = new HtmlString(newUserCommand.Password)
            };

            return context;
        }

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
    }
}
