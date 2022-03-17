using System.Globalization;

namespace Cofoundry.Domain;

/// <summary>
/// Service abstraction over the culture of the current request.
/// </summary>
public interface ICultureContextService
{
    /// <summary>
    /// Gets the CultureInfo used by the current request.
    /// </summary>
    CultureInfo GetCurrent();

    /// <summary>
    /// Sets the current thread culture and UI culture.
    /// </summary>
    /// <param name="ietfLanguageTag">An IETF language tag to set the current thread culture to e.g. 'en-US' or 'es'.</param>
    void SetCurrent(string ietfLanguageTag);
}
