namespace Cofoundry.Domain;

/// <summary>
/// Extensions for filtering collections of <see cref="PageRoute"/>.
/// </summary>
public static class PageRouteExtensions
{
    extension(IEnumerable<PageRoute> routes)
    {
        public IEnumerable<PageRoute> FilterByDirectory(
            PageDirectoryRoute directory,
            int? localeId = null)
        {
            return routes
                .Where(r => (r.PageDirectory == null && directory == null) || r.PageDirectory == directory)
                .Where(r => (r.Locale == null && !localeId.HasValue) || (r.Locale != null && r.Locale.LocaleId == localeId));
        }
    }
}
