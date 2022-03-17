using Cofoundry.Core.Web;

namespace Cofoundry.Domain.MailTemplates.Internal;

/// <inheritdoc/>
public class UserMailTemplateInitializer : IUserMailTemplateInitializer
{
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
    private readonly IQueryExecutor _queryExecutor;
    private readonly ISiteUrlResolver _siteUrlResolver;
    private readonly AdminSettings _adminSettings;

    public UserMailTemplateInitializer(
        IUserAreaDefinitionRepository userAreaDefinitionRepository,
        IQueryExecutor queryExecutor,
        ISiteUrlResolver siteUrlResolver,
        AdminSettings adminSettings
        )
    {
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
        _queryExecutor = queryExecutor;
        _siteUrlResolver = siteUrlResolver;
        _adminSettings = adminSettings;
    }

    public async Task Initialize<TTemplate>(UserSummary user, TTemplate mailTemplate)
        where TTemplate : UserMailTemplateBase
    {
        if (mailTemplate == null) throw new ArgumentNullException(nameof(mailTemplate));
        if (user == null) throw new ArgumentNullException(nameof(user));

        mailTemplate.User = user;
        mailTemplate.ApplicationName = await GetApplicationNameAsync();

        if (mailTemplate is IMailTemplateWithSignInUrl mailTemplateWithSignInUrl)
        {
            mailTemplateWithSignInUrl.SignInUrl = GetSignInUrl(user);
        }

        if (user.UserArea.UserAreaCode == CofoundryAdminUserArea.Code)
        {
            mailTemplate.LayoutFile = AdminMailTemplatePath.LayoutPath;
            mailTemplate.ViewFile = AdminMailTemplatePath.TemplateView(mailTemplate.GetType().Name);
        }
    }

    public string GetSignInUrl(UserSummary user)
    {
        string signInPath;

        if (user.UserArea.UserAreaCode == CofoundryAdminUserArea.Code)
        {
            signInPath = "/" + _adminSettings.DirectoryName;
        }
        else
        {
            var options = _userAreaDefinitionRepository.GetRequiredByCode(user.UserArea.UserAreaCode);
            signInPath = options.SignInPath;
        }

        return _siteUrlResolver.MakeAbsolute(signInPath);
    }

    public async Task<string> GetApplicationNameAsync()
    {
        var query = new GetSettingsQuery<GeneralSiteSettings>();
        var result = await _queryExecutor.ExecuteAsync(query);

        return result?.ApplicationName;
    }
}
