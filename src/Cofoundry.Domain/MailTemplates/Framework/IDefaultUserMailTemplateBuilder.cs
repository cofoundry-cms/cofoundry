namespace Cofoundry.Domain.MailTemplates.Internal
{
    /// <summary>
    /// Used construct the default mail templates that are used by Cofoundry 
    /// to send email for actions such as creating new users and resetting password 
    /// etc.
    /// </summary>
    public interface IDefaultUserMailTemplateBuilder<T> : IUserMailTemplateBuilder
        where T : IUserAreaDefinition
    {
    }
}