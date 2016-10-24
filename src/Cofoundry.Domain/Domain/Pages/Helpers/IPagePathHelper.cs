using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IPagePathHelper
    {
        /// <summary>
        /// Standardises a page path ensuring it 
        /// - starts with a slash
        /// - doesn't end with a trailing slash
        /// </summary>
        /// <param name="path">Path to standadise</param>
        string StandardisePath(string path);

        /// <summary>
        /// Standardises a page path ensuring it 
        /// - starts with a slash
        /// - doesn't end with a trailing slash
        /// - (optionally) does not contain the locale.
        /// </summary>
        /// <param name="path">Path to standadise</param>
        /// <param name="currentLocale">Locale of the path to remove if present.</param>
        string StandardisePathWithoutLocale(string path, ActiveLocale currentLocale);
    }
}
