using Cofoundry.Core.Mail;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.MailTemplates.Internal;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Resets a users password to a randomly generated temporary value
/// and sends it in a mail a notification to the user. The password
/// will need to be changed at first sign in (if the user area supports 
/// it). This is designed to be used from an admin screen rather than 
/// a self-service reset which can be done via 
/// <see cref="InitiateUserAccountRecoveryViaEmailCommand"/>.
/// </summary>
public class ResetUserPasswordCommandHandler
    : ICommandHandler<ResetUserPasswordCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IMailService _mailService;
    private readonly IDomainRepository _domainRepository;
    private readonly IUserMailTemplateBuilderContextFactory _userMailTemplateBuilderContextFactory;
    private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly UserCommandPermissionsHelper _userCommandPermissionsHelper;
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
    private readonly IPasswordCryptographyService _passwordCryptographyService;
    private readonly IPasswordGenerationService _passwordGenerationService;
    private readonly IUserSecurityStampUpdateHelper _userSecurityStampUpdateHelper;
    private readonly IUserContextCache _userContextCache;
    private readonly IUserSummaryMapper _userSummaryMapper;
    private readonly IMessageAggregator _messageAggregator;

    public ResetUserPasswordCommandHandler(
        CofoundryDbContext dbContext,
        IMailService mailService,
        IDomainRepository domainRepository,
        IUserMailTemplateBuilderContextFactory userMailTemplateBuilderContextFactory,
        IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
        IPermissionValidationService permissionValidationService,
        UserCommandPermissionsHelper userCommandPermissionsHelper,
        IUserAreaDefinitionRepository userAreaDefinitionRepository,
        IPasswordCryptographyService passwordCryptographyService,
        IPasswordGenerationService passwordGenerationService,
        IUserSecurityStampUpdateHelper userSecurityStampUpdateHelper,
        IUserContextCache userContextCache,
        IUserSummaryMapper userSummaryMapper,
        IMessageAggregator messageAggregator
        )
    {
        _dbContext = dbContext;
        _mailService = mailService;
        _domainRepository = domainRepository;
        _userMailTemplateBuilderContextFactory = userMailTemplateBuilderContextFactory;
        _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
        _permissionValidationService = permissionValidationService;
        _userCommandPermissionsHelper = userCommandPermissionsHelper;
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
        _passwordCryptographyService = passwordCryptographyService;
        _passwordGenerationService = passwordGenerationService;
        _userSecurityStampUpdateHelper = userSecurityStampUpdateHelper;
        _userContextCache = userContextCache;
        _userSummaryMapper = userSummaryMapper;
        _messageAggregator = messageAggregator;
    }

    public async Task ExecuteAsync(ResetUserPasswordCommand command, IExecutionContext executionContext)
    {
        var user = await GetUserAsync(command.UserId);
        await ValidatePermissionsAsync(user, executionContext);
        ValidateUserArea(user.UserAreaCode);

        var options = _userAreaDefinitionRepository.GetOptionsByCode(user.UserAreaCode);
        var temporaryPassword = _passwordGenerationService.Generate(options.Password.MinLength);

        var hashResult = _passwordCryptographyService.CreateHash(temporaryPassword);
        user.Password = hashResult.Hash;
        user.PasswordHashVersion = hashResult.HashVersion;
        user.RequirePasswordChange = true;
        user.LastPasswordChangeDate = executionContext.ExecutionDate;
        _userSecurityStampUpdateHelper.Update(user);

        using (var scope = _domainRepository.Transactions().CreateScope())
        {
            await _dbContext.SaveChangesAsync();
            await SendNotificationAsync(user, temporaryPassword, executionContext);

            scope.QueueCompletionTask(() => OnTransactionComplete(user));
            await scope.CompleteAsync();
        }
    }

    private async Task OnTransactionComplete(User user)
    {
        _userContextCache.Clear();

        await _userSecurityStampUpdateHelper.OnTransactionCompleteAsync(user);

        await _messageAggregator.PublishAsync(new UserPasswordResetMessage()
        {
            UserAreaCode = user.UserAreaCode,
            UserId = user.UserId
        });
    }

    private Task<User> GetUserAsync(int userId)
    {
        var user = _dbContext
            .Users
            .IncludeForSummary()
            .FilterById(userId)
            .FilterCanSignIn()
            .SingleOrDefaultAsync();

        EntityNotFoundException.ThrowIfNull(user, userId);

        return user;
    }

    private void ValidateUserArea(string userAreaCode)
    {
        var userArea = _userAreaDefinitionRepository.GetRequiredByCode(userAreaCode);

        if (!userArea.AllowPasswordSignIn)
        {
            throw new InvalidOperationException($"Cannot reset the password because the {userArea.Name} user area does not allow password sign in.");
        }

        if (!userArea.UseEmailAsUsername)
        {
            throw new InvalidOperationException($"Cannot reset the password because the {userArea.Name} user area does not require email addresses.");
        }
    }

    private async Task SendNotificationAsync(User user, string temporaryPassword, IExecutionContext executionContext)
    {
        var userSummary = _userSummaryMapper.Map(user);
        var context = _userMailTemplateBuilderContextFactory.CreatePasswordResetContext(userSummary, temporaryPassword);
        var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(user.UserAreaCode);
        var mailTemplate = await mailTemplateBuilder.BuildPasswordResetTemplateAsync(context);

        // Null template means don't send a notification
        if (mailTemplate == null) return;

        await _mailService.SendAsync(user.Email, mailTemplate);
    }

    public async Task ValidatePermissionsAsync(User user, IExecutionContext executionContext)
    {
        if (user.UserId == executionContext.UserContext.UserId)
        {
            throw new NotPermittedException("A user cannot reset the password on their own user account.");
        }

        var userArea = _userAreaDefinitionRepository.GetRequiredByCode(user.UserAreaCode);
        if (userArea is CofoundryAdminUserArea)
        {
            _permissionValidationService.EnforcePermission(new CofoundryUserResetPasswordPermission(), executionContext.UserContext);
        }
        else
        {
            _permissionValidationService.EnforcePermission(new NonCofoundryUserResetPasswordPermission(), executionContext.UserContext);
        }

        await _userCommandPermissionsHelper.ThrowIfCannotManageSuperAdminAsync(user, executionContext);
    }
}
