using System.Globalization;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// A culture factory that allows the creation and registration of custom cultures.
/// </summary>
public class CultureFactory : ICultureFactory
{
    public CultureInfo Create(string languageTag)
    {
        if (string.IsNullOrEmpty(languageTag))
        {
            throw new ArgumentException("Cannot create a culture with an empty language tag.", languageTag);
        }

        return new CultureInfo(languageTag);
    }
}
