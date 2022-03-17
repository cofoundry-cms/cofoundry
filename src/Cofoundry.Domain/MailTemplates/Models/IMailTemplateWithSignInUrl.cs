using Cofoundry.Core.Mail;

namespace Cofoundry.Domain.MailTemplates.Internal;

/// <summary>
/// Used to mark up a template that contains a sign in url so it can be initialized
/// with <see cref="IUserMailTemplateInitializer"/>.
/// </summary>
public interface IMailTemplateWithSignInUrl : IMailTemplate
{
    /// <summary>
    /// The absolute sign in page url e.g. "https://www.example.com/members/sign-in".
    /// </summary>
    string SignInUrl { get; set; }
}
