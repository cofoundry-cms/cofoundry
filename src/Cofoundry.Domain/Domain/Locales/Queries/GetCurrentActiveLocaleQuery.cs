namespace Cofoundry.Domain;

/// <summary>
/// Gets the locale associated with the current thread culture (uses ICultureContextService)
/// </summary>
public class GetCurrentActiveLocaleQuery : IQuery<ActiveLocale>
{
}
