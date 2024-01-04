using System.Globalization;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="ICultureFactory"/>.
/// </summary>
public class CultureFactory : ICultureFactory
{
    public CultureInfo Create(string languageTag)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(languageTag);

        return new CultureInfo(languageTag);
    }
}
