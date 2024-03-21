namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="ICultureContextService"/>.
/// </summary>
public class CultureContextService : ICultureContextService
{
    private readonly ICultureFactory _cultureFactory;

    public CultureContextService(
        ICultureFactory cultureFactory
        )
    {
        _cultureFactory = cultureFactory;
    }

    /// <inheritdoc/>
    public CultureInfo GetCurrent()
    {
        return CultureInfo.CurrentCulture;
    }

    /// <inheritdoc/>
    public void SetCurrent(string ietfLanguageTag)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(ietfLanguageTag);

        var culture = _cultureFactory.Create(ietfLanguageTag);

        if (culture != null)
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }
    }
}
