namespace Cofoundry.Domain;

/// <summary>
/// Used in the UpdatePageTemplateRegistrationCommandHandler when there's
/// a problem with the registration of template files that cannot be 
/// resolved automatically.
/// </summary>
public class PageTemplateRegistrationException : Exception
{
    public PageTemplateRegistrationException()
    {
    }

    public PageTemplateRegistrationException(string message)
        : base(message)
    {
    }
}
