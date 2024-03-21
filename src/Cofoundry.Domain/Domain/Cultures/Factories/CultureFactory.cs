namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="ICultureFactory"/>.
/// </summary>
public class CultureFactory : ICultureFactory
{
    public CultureInfo Create(string languageTag)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(languageTag);

        return new CultureInfo(languageTag);
    }
}
