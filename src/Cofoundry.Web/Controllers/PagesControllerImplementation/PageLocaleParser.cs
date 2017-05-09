using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Web
{
    /// <summary>
    /// Used to parse a locale from a page url e.g. '/ca-en/mypage' is Canadian english locale
    /// </summary>
    public class PageLocaleParser : IPageLocaleParser
    {
        private readonly IQueryExecutor _queryExecutor;

        public PageLocaleParser(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        /// <summary>
        /// Parses a locale string from the site path and checks if
        /// it is an active locale, returning the ActiveLocale object if
        /// found.
        /// </summary>
        public async Task<ActiveLocale> ParseLocaleAsync(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;

            ActiveLocale locale = null;
            string localeStr;

            if (path.Contains("/"))
            {
                localeStr = path.Split('/').First();
            }
            else
            {
                localeStr = path;
            }

            // Check the first part of the string matches the format for a locale
            if (Regex.Match(localeStr, @"^[a-zA-Z]{2}(-[a-zA-Z]{2})?$", RegexOptions.IgnoreCase).Success)
            {
                var query = new GetActiveLocaleByIETFLanguageTagQuery(localeStr);
                locale = await _queryExecutor.ExecuteAsync(query);
            }

            return locale;
        }
    }
}
