﻿namespace Cofoundry.Domain;

public static class PageRouteExtensions
{
    public static IEnumerable<PageRoute> FilterByDirectory(
        this IEnumerable<PageRoute> routes,
        PageDirectoryRoute directory,
        int? localeId = null
        )
    {
        return routes
            .Where(r => (r.PageDirectory == null && directory == null) || r.PageDirectory == directory)
            .Where(r => (r.Locale == null && !localeId.HasValue) || (r.Locale != null && r.Locale.LocaleId == localeId))
            ;
    }
}
