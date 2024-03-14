using System.Text.RegularExpressions;

namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="IPageLocaleParser"/>.
/// </summary>
public partial class PageLocaleParser : IPageLocaleParser
{
    private readonly IQueryExecutor _queryExecutor;

    public PageLocaleParser(
        IQueryExecutor queryExecutor
        )
    {
        _queryExecutor = queryExecutor;
    }

    /// <inheritdoc/>
    public async Task<ActiveLocale?> ParseLocaleAsync(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        ActiveLocale? locale = null;
        string localeStr;

        if (path.Contains('/'))
        {
            localeStr = path.Split('/').First();
        }
        else
        {
            localeStr = path;
        }

        // Check the first part of the string matches the format for a locale
        if (LocaleRegex().Match(localeStr).Success)
        {
            var query = new GetActiveLocaleByIETFLanguageTagQuery(localeStr);
            locale = await _queryExecutor.ExecuteAsync(query);
        }

        return locale;
    }

    [GeneratedRegex(@"^[a-zA-Z]{2}(-[a-zA-Z]{2})?$", RegexOptions.IgnoreCase)]
    private static partial Regex LocaleRegex();
}
