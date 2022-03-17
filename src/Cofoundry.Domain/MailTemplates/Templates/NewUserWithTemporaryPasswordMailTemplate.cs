using Cofoundry.Domain.MailTemplates.Internal;
using Microsoft.AspNetCore.Html;

namespace Cofoundry.Domain.MailTemplates;

/// <summary>
/// Template for the email sent to a new user when their account has
/// been created with a temporary password. This default version of the 
/// template is used for all user areas except the Cofoundry admin 
/// user area.
/// </summary>
public class NewUserWithTemporaryPasswordMailTemplate : UserMailTemplateBase, IMailTemplateWithSignInUrl
{
    public NewUserWithTemporaryPasswordMailTemplate()
    {
        LayoutFile = DefaultMailTemplatePath.LayoutPath;
        ViewFile = DefaultMailTemplatePath.TemplateView(nameof(NewUserWithTemporaryPasswordMailTemplate));
        SubjectFormat = "{0}: Your account has been created";
    }

    /// <summary>
    /// The newly created user.
    /// </summary>
    public override UserSummary User { get; set; }

    /// <summary>
    /// The temporary password that the user can use to log in to 
    /// the site.
    /// </summary>
    public IHtmlContent TemporaryPassword { get; set; }

    /// <summary>
    /// The absolute sign in page url e.g. "https://www.example.com/members/sign-in".
    /// </summary>
    public string SignInUrl { get; set; }
}
