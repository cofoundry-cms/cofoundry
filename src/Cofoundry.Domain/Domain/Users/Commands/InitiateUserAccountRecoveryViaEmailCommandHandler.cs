﻿using Cofoundry.Core.ExecutionDurationRandomizer;
using Cofoundry.Core.Mail;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.MailTemplates.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// <para>
/// Initiates an email-based account recovery (AKA "forgot password") request, sending a 
/// notification to the user with a url to an account recovery form. This command
/// is designed for self-service password reset so the password is not changed 
/// until the form has been completed. 
/// </para>
/// <para>
/// Requests are logged and validated to prevent too many account recovery
/// attempts being initiated in a set period of time.
/// </para>
/// </summary>
public class InitiateUserAccountRecoveryViaEmailCommandHandler
    : ICommandHandler<InitiateUserAccountRecoveryViaEmailCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IDomainRepository _domainRepository;
    private readonly IMailService _mailService;
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
    private readonly IUserMailTemplateBuilderContextFactory _userMailTemplateBuilderContextFactory;
    private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
    private readonly IUserDataFormatter _userDataFormatter;
    private readonly IMessageAggregator _messageAggregator;
    private readonly IExecutionDurationRandomizerScopeManager _executionDurationRandomizerScopeManager;
    private readonly IUserSummaryMapper _userSummaryMapper;

    public InitiateUserAccountRecoveryViaEmailCommandHandler(
        CofoundryDbContext dbContext,
        IDomainRepository domainRepository,
        IMailService mailService,
        IUserAreaDefinitionRepository userAreaDefinitionRepository,
        IUserMailTemplateBuilderContextFactory userMailTemplateBuilderContextFactory,
        IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
        IUserDataFormatter userDataFormatter,
        IMessageAggregator messageAggregator,
        IExecutionDurationRandomizerScopeManager taskDurationRandomizerScopeManager,
        IUserSummaryMapper userSummaryMapper
        )
    {
        _dbContext = dbContext;
        _domainRepository = domainRepository;
        _mailService = mailService;
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
        _userMailTemplateBuilderContextFactory = userMailTemplateBuilderContextFactory;
        _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
        _userDataFormatter = userDataFormatter;
        _messageAggregator = messageAggregator;
        _executionDurationRandomizerScopeManager = taskDurationRandomizerScopeManager;
        _userSummaryMapper = userSummaryMapper;
    }

    public async Task ExecuteAsync(InitiateUserAccountRecoveryViaEmailCommand command, IExecutionContext executionContext)
    {
        var options = _userAreaDefinitionRepository.GetOptionsByCode(command.UserAreaCode).AccountRecovery;
        await using (_executionDurationRandomizerScopeManager.Create(options.ExecutionDuration))
        {
            await ExecuteInternalAsync(command, executionContext);
        }
    }

    private async Task ExecuteInternalAsync(InitiateUserAccountRecoveryViaEmailCommand command, IExecutionContext executionContext)
    {
        ValidateUserArea(command.UserAreaCode);
        var options = _userAreaDefinitionRepository.GetOptionsByCode(command.UserAreaCode).AccountRecovery;

        var user = await GetUserAsync(command);
        if (user == null)
        {
            return;
        }

        using (var scope = _domainRepository.Transactions().CreateScope())
        {
            var addAuthorizedTaskCommand = await GenerateTokenAsync(user, options, executionContext);
            await SendNotificationAsync(addAuthorizedTaskCommand, user);

            scope.QueueCompletionTask(() => OnTransactionComplete(user, addAuthorizedTaskCommand));
            await scope.CompleteAsync();
        }
    }

    private async Task OnTransactionComplete(UserSummary user, AddAuthorizedTaskCommand addAuthorizedTaskCommand)
    {
        ArgumentException.ThrowIfNullOrEmpty(addAuthorizedTaskCommand.OutputToken);

        await _messageAggregator.PublishAsync(new UserAccountRecoveryInitiatedMessage()
        {
            AuthorizedTaskId = addAuthorizedTaskCommand.OutputAuthorizedTaskId,
            UserAreaCode = user.UserArea.UserAreaCode,
            UserId = user.UserId,
            Token = addAuthorizedTaskCommand.OutputToken
        });
    }

    private void ValidateUserArea(string userAreaCode)
    {
        var userArea = _userAreaDefinitionRepository.GetRequiredByCode(userAreaCode);

        if (!userArea.AllowPasswordSignIn)
        {
            throw new InvalidOperationException($"Cannot reset the password because the {userArea.Name} user area does not allow password sign in.");
        }
    }

    private async Task<UserSummary?> GetUserAsync(InitiateUserAccountRecoveryViaEmailCommand command)
    {
        var username = _userDataFormatter.UniquifyUsername(command.UserAreaCode, command.Username);
        var dbUser = await _dbContext
            .Users
            .Include(u => u.Role)
            .FilterByUserArea(command.UserAreaCode)
            .FilterCanSignIn()
            .SingleOrDefaultAsync(u => u.Username == username);

        var user = _userSummaryMapper.Map(dbUser);

        return user;
    }

    private async Task<AddAuthorizedTaskCommand> GenerateTokenAsync(
        UserSummary user,
        AccountRecoveryOptions options,
        IExecutionContext executionContext
        )
    {
        if (string.IsNullOrWhiteSpace(user.Email))
        {
            throw new InvalidOperationException($"Cannot reset the password because user {user.UserId} does not have an email address.");
        }

        var command = new AddAuthorizedTaskCommand()
        {
            AuthorizedTaskTypeCode = UserAccountRecoveryAuthorizedTaskType.Code,
            UserId = user.UserId,
            ExpireAfter = options.ExpireAfter
        };

        // Authorized tasks supports rate limits without a time window, but password 
        // resets require a quantity and a window to set a rate limit 
        if (options.InitiationRateLimit != null && options.InitiationRateLimit.HasValidQuantityAndWindow())
        {
            command.RateLimit = options.InitiationRateLimit;
        }

        try
        {
            await _domainRepository
                .WithContext(executionContext)
                .ExecuteCommandAsync(command);
        }
        catch (ValidationErrorException ex)
        {
            if (ex.ErrorCode == AuthorizedTaskValidationErrors.Create.RateLimitExceeded.ErrorCode)
            {
                // Use a more specific error message
                UserValidationErrors.AccountRecovery.Initiation.RateLimitExceeded.Throw();
            }

            throw;
        }

        return command;
    }

    private async Task<string> SendNotificationAsync(
        AddAuthorizedTaskCommand addAuthorizedTaskCommand,
        UserSummary user
        )
    {
        EntityInvalidOperationException.ThrowIfNull(user, user.Email);
        ArgumentException.ThrowIfNullOrEmpty(addAuthorizedTaskCommand.OutputToken);

        var context = _userMailTemplateBuilderContextFactory.CreateAccountRecoveryContext(user, addAuthorizedTaskCommand.OutputToken);
        var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(user.UserArea.UserAreaCode);
        var mailTemplate = await mailTemplateBuilder.BuildAccountRecoveryTemplateAsync(context);

        // Null template means don't send a notification
        if (mailTemplate != null)
        {
            await _mailService.SendAsync(user.Email, mailTemplate);
        }

        return context.Token;
    }
}
