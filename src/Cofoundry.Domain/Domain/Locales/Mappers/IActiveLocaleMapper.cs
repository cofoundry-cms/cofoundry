using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple mapper for mapping to ActiveLocale objects.
    /// </summary>
    public interface IActiveLocaleMapper
    {
        /// <summary>
        /// Maps an EF Locale record from the db into an ActiveLocale 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbLocale">Locale record from the database.</param>
        ActiveLocale Map(Locale dbLocale);
    }
}
