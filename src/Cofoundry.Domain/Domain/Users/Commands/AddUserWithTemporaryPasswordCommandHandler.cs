using Cofoundry.Core.Data;
using Cofoundry.Core.Mail;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.MailTemplates.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Adds a new user and sends a notification containing a generated 
/// password which must be changed at first sign in.
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
    private readonly IUserMailTemplateBuilderContextFactory _userMailTemplateBuilderContextFactory;
    private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
    private readonly ITransactionScopeManager _transactionScopeManager;
    private readonly IUserDataFormatter _userDataFormatter;
    private readonly IPasswordPolicyService _passwordPolicyService;

    public AddUserWithTemporaryPasswordCommandHandler(
        CofoundryDbContext dbContext,
        ICommandExecutor commandExecutor,
        IPasswordGenerationService passwordGenerationService,
        IMailService mailService,
        IQueryExecutor queryExecutor,
        IUserMailTemplateBuilderContextFactory userMailTemplateBuilderContextFactory,
        IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
        IUserAreaDefinitionRepository userAreaDefinitionRepository,
        ITransactionScopeManager transactionScopeManager,
        IUserDataFormatter userDataFormatter,
        IPasswordPolicyService passwordPolicyService
        )
    {
        _dbContext = dbContext;
        _commandExecutor = commandExecutor;
        _passwordGenerationService = passwordGenerationService;
        _mailService = mailService;
        _queryExecutor = queryExecutor;
        _userMailTemplateBuilderContextFactory = userMailTemplateBuilderContextFactory;
        _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
        _transactionScopeManager = transactionScopeManager;
        _userDataFormatter = userDataFormatter;
        _passwordPolicyService = passwordPolicyService;
    }

    public async Task ExecuteAsync(AddUserWithTemporaryPasswordCommand command, IExecutionContext executionContext)
    {
        ValidateUserArea(command);
        Normalize(command);

        using (var scope = _transactionScopeManager.Create(_dbContext))
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
        var email = _userDataFormatter.NormalizeEmail(command.UserAreaCode, command.Email);

        if (string.IsNullOrEmpty(email))
        {
            throw ValidationErrorException.CreateWithProperties("Email address is invalid", nameof(command.Email));
        }

        command.Email = email;
    }

    private void ValidateUserArea(AddUserWithTemporaryPasswordCommand command)
    {
        var userArea = _userAreaDefinitionRepository.GetRequiredByCode(command.UserAreaCode);
        if (!userArea.AllowPasswordSignIn)
        {
            throw new InvalidOperationException(nameof(AddUserWithTemporaryPasswordCommand) + " must be used with a user area that supports password based sign in.");
        }
    }

    private AddUserCommand MapCommand(AddUserWithTemporaryPasswordCommand command)
    {
        // The password policy should be configured with the definitive min-length
        // as an attribute, but otherwise fall-back to the configured option
        var passwordPolicy = _passwordPolicyService.GetDescription(command.UserAreaCode);
        var minLengthAttribute = passwordPolicy.Attributes.GetValueOrDefault(PasswordPolicyAttributes.MinLength);
        var options = _userAreaDefinitionRepository.GetOptionsByCode(command.UserAreaCode);
        var minLength = IntParser.ParseOrDefault(minLengthAttribute, options.Password.MinLength);

        var newUserCommand = new AddUserCommand()
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            DisplayName = command.DisplayName,
            Email = command.Email,
            Username = command.Username,
            Password = _passwordGenerationService.Generate(minLength),
            RequirePasswordChange = true,
            UserAreaCode = command.UserAreaCode,
            RoleId = command.RoleId,
            RoleCode = command.RoleCode
        };

        return newUserCommand;
    }

    private async Task SendNotificationAsync(AddUserCommand newUserCommand, IExecutionContext executionContext)
    {
        if (string.IsNullOrWhiteSpace(newUserCommand.Password))
        {
            throw new InvalidOperationException($"{nameof(newUserCommand)}.{nameof(newUserCommand.Password)} should not be empty.");
        }

        if (string.IsNullOrWhiteSpace(newUserCommand.Email))
        {
            throw new InvalidOperationException($"{nameof(newUserCommand)}.{nameof(newUserCommand.Email)} should not be empty.");
        }

        var query = new GetUserSummaryByIdQuery(newUserCommand.OutputUserId);
        var user = await _queryExecutor.ExecuteAsync(query, executionContext);
        EntityNotFoundException.ThrowIfNull(user, newUserCommand.OutputUserId);

        // Send mail notification
        var context = _userMailTemplateBuilderContextFactory.CreateNewUserWithTemporaryPasswordContext(user, newUserCommand.Password);
        var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(newUserCommand.UserAreaCode);
        var mailTemplate = await mailTemplateBuilder.BuildNewUserWithTemporaryPasswordTemplateAsync(context);

        // Null template means don't send a notification
        if (mailTemplate == null)
        {
            return;
        }

        await _mailService.SendAsync(newUserCommand.Email, mailTemplate);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(AddUserWithTemporaryPasswordCommand command)
    {
        if (command.UserAreaCode == CofoundryAdminUserArea.Code)
        {
            yield return new CofoundryUserCreatePermission();
        }
        else
        {
            yield return new NonCofoundryUserCreatePermission();
        }
    }
}
