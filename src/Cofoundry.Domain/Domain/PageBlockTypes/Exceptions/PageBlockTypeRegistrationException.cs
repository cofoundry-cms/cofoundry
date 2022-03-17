namespace Cofoundry.Domain;

/// <summary>
/// Used in RegisterPageBlockTypesCommandHandler when there's
/// a problem with the registration of page block types that cannot be 
/// resolved automatically.
/// </summary>
public class PageBlockTypeRegistrationException : Exception
{
    public PageBlockTypeRegistrationException()
    {
    }

    public PageBlockTypeRegistrationException(string message)
        : base(message)
    {
    }
}
