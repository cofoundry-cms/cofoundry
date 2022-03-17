using System.Globalization;

namespace Cofoundry.Domain;

/// <summary>
/// A culture factory that allows the creation and registration of custom cultures.
/// </summary>
public interface ICultureFactory
{
    CultureInfo Create(string languageTag);
}
