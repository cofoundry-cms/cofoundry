using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
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
}
