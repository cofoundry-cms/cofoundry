using Cofoundry.Core.Mail;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Internal;
using Cofoundry.Domain.MailTemplates.Internal;

namespace Cofoundry.Domain;

/// <inheritdoc/>
public class PasswordUpdateCommandHelper : IPasswordUpdateCommandHelper
{
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
    private readonly IPermissionValidationService _permissionValidationService;
    private readonly IPasswordCryptographyService _passwordCryptographyService;
    private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
    private readonly IUserMailTemplateBuilderContextFactory _userMailTemplateBuilderContextFactory;
    private readonly IUserSummaryMapper _userSummaryMapper;
    private readonly IMailService _mailService;

    public PasswordUpdateCommandHelper(
        IUserAreaDefinitionRepository userAreaDefinitionRepository,
        IPermissionValidationService permissionValidationService,
        IPasswordCryptographyService passwordCryptographyService,
        IMailService mailService,
        IUserMailTemplateBuilderContextFactory userMailTemplateBuilderContextFactory,
        IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
        IUserSummaryMapper userSummaryMapper
        )
    {
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
        _passwordCryptographyService = passwordCryptographyService;
        _permissionValidationService = permissionValidationService;
        _mailService = mailService;
        _userMailTemplateBuilderContextFactory = userMailTemplateBuilderContextFactory;
        _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
        _userSummaryMapper = userSummaryMapper;
    }

    public void ValidateUserArea(IUserAreaDefinition userArea)
    {
        if (!userArea.AllowPasswordSignIn)
        {
            throw new InvalidOperationException("Cannot update the password to account in a user area that does not allow password sign in.");
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
        ArgumentNullException.ThrowIfNull(user);

        var options = _userAreaDefinitionRepository.GetOptionsByCode(user.UserAreaCode).Password;
        if (!options.SendNotificationOnUpdate) return;

        var userSummary = _userSummaryMapper.Map(user);
        var context = _userMailTemplateBuilderContextFactory.CreatePasswordChangedContext(userSummary);
        var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(user.UserAreaCode);
        var mailTemplate = await mailTemplateBuilder.BuildPasswordChangedTemplateAsync(context);

        // Null template means don't send a notification
        if (mailTemplate == null) return;

        await _mailService.SendAsync(user.Email, mailTemplate);
    }
}
