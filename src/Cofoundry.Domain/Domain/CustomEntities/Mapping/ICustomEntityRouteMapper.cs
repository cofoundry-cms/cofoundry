using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Simple mapper for mapping to CustomEntityRoute objects.
/// </summary>
public interface ICustomEntityRouteMapper
{
    /// <summary>
    /// Maps an EF CustomEntity record from the db into a CustomEntityRoute object. If the
    /// db record is null then null is returned.
    /// </summary>
    /// <param name="dbCustomEntity">CustomEntity record from the database.</param>
    /// <param name="locale">Locale to map to the object.</param>
    CustomEntityRoute Map(
        CustomEntity dbCustomEntity,
        ActiveLocale locale
        );
}
