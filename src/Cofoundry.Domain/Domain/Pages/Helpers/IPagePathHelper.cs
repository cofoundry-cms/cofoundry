using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public interface IPagePathHelper
    {
        /// <summary>
        /// Standardizes a page path ensuring it 
        /// - starts with a slash
        /// - doesn't end with a trailing slash
        /// </summary>
        /// <param name="path">Path to standardize.</param>
        string StandardizePath(string path);

        /// <summary>
        /// Standardizes a page path ensuring it 
        /// - starts with a slash
        /// - doesn't end with a trailing slash
        /// - (optionally) does not contain the locale
        /// </summary>
        /// <param name="path">Path to standardize.</param>
        /// <param name="currentLocale">Locale of the path to remove if present.</param>
        string StandardizePathWithoutLocale(string path, ActiveLocale currentLocale);
    }
}
