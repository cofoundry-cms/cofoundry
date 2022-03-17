namespace Cofoundry.Domain.MailTemplates.Internal;

/// <summary>
/// Used to intialize common properties on a template that 
/// inherits from <see cref="UserMailTemplateBase"/>.
/// </summary>
public interface IUserMailTemplateInitializer
{
    /// <summary>
    /// Intializes common properties on the <paramref name="mailTemplate"/>
    /// instance.
    /// </summary>
    Task Initialize<TTemplate>(UserSummary user, TTemplate mailTemplate)
        where TTemplate : UserMailTemplateBase;
}
