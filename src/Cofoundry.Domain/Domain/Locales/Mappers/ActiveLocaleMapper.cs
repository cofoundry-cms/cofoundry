using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to ActiveLocale objects.
    /// </summary>
    public class ActiveLocaleMapper : IActiveLocaleMapper
    {
        /// <summary>
        /// Maps an EF Locale record from the db into an ActiveLocale 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbLocale">Locale record from the database.</param>
        public ActiveLocale Map(Locale dbLocale)
        {
            if (dbLocale == null) return null;
            
            var locale = new ActiveLocale()
            {
                IETFLanguageTag = dbLocale.IETFLanguageTag,
                LocaleId = dbLocale.LocaleId,
                Name = dbLocale.LocaleName
            };

            return locale;
        }
    }
}
