using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// Used to parse a locale from a page url e.g. '/ca-en/mypage' is Canadian english locale
    /// </summary>
    public interface IPageLocaleParser
    {
        /// <summary>
        /// Parses a locale string from the site path and checks if
        /// it is an active locale, returning the ActiveLocale object if
        /// found.
        /// </summary>
        Task<ActiveLocale> ParseLocaleAsync(string path);
    }
}
