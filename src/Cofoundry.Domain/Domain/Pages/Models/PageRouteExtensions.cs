using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public static class PageRouteExtensions
    {
        public static IEnumerable<PageRoute> FilterByDirectory(this IEnumerable<PageRoute> routes, PageDirectoryRoute directory, int? localeId = null)
        {
            return routes
                .Where(r => (r.PageDirectory == null && directory == null) || r.PageDirectory == directory)
                .Where(r => (r.Locale == null && !localeId.HasValue) || r.Locale.LocaleId == localeId)
                ;
        }
    }
}
