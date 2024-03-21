using System.Text.RegularExpressions;

namespace Cofoundry.Domain;

/// <summary>
/// A routing rule that uses only the UrlSlug property to match the
/// custom entity. This rule requires the custom entity definition
/// forces url slugs to be unique.
/// </summary>
public partial class UrlSlugCustomEntityRoutingRule : ICustomEntityRoutingRule
{
    /// <inheritdoc/>
    public string RouteFormat => "{UrlSlug}";

    /// <inheritdoc/>
    public int Priority => (int)RoutingRulePriority.Normal;

    /// <inheritdoc/>
    public bool RequiresUniqueUrlSlug => true;

    /// <inheritdoc/>
    public bool MatchesRule(string url, PageRoute pageRoute)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(url);
        ArgumentNullException.ThrowIfNull(pageRoute);

        var slugUrlPart = GetRoutingPart(url, pageRoute);
        if (string.IsNullOrEmpty(slugUrlPart))
        {
            return false;
        }

        var isMatch = SlugCaseInsensitiveRegex().IsMatch(slugUrlPart);

        return isMatch;
    }

    /// <inheritdoc/>
    public IQuery<CustomEntityRoute?> ExtractRoutingQuery(string url, PageRoute pageRoute)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(url);
        ArgumentNullException.ThrowIfNull(pageRoute);
        ArgumentNullException.ThrowIfNull(pageRoute.CustomEntityDefinitionCode);

        if (!MatchesRule(url, pageRoute))
        {
            throw new ArgumentException(nameof(url) + $" does not match the specified page route. {nameof(ExtractRoutingQuery)} can only be called after a successful page route match.", nameof(url));
        }

        var slugUrlPart = GetRoutingPart(url, pageRoute);
        if (string.IsNullOrEmpty(slugUrlPart))
        {
            throw new InvalidOperationException($"{nameof(slugUrlPart)} is not expected to be null when {nameof(MatchesRule)} is true. Url: {url}, pageRoute: {pageRoute.FullUrlPath}");
        }

        var query = new GetCustomEntityRouteByPathQuery()
        {
            CustomEntityDefinitionCode = pageRoute.CustomEntityDefinitionCode,
            UrlSlug = slugUrlPart
        };

        if (pageRoute.Locale != null)
        {
            query.LocaleId = pageRoute.Locale.LocaleId;
        }

        return query;
    }

    /// <inheritdoc/>
    public string MakeUrl(PageRoute pageRoute, CustomEntityRoute entityRoute)
    {
        ArgumentNullException.ThrowIfNull(pageRoute);
        ArgumentNullException.ThrowIfNull(entityRoute);

        return pageRoute.FullUrlPath.Replace(RouteFormat, entityRoute.UrlSlug);
    }

    /// <summary>
    /// Extracts the custom entity routing part of the path from 
    /// a <paramref name="url"/> e.g. url "/my-path/123" with Pageroute 
    /// "/my-path/{id}" will return "123".
    /// </summary>
    private string? GetRoutingPart(string url, PageRoute pageRoute)
    {
        if (!pageRoute.FullUrlPath.Contains(RouteFormat, StringComparison.Ordinal))
        {
            return null;
        }

        var pathRoot = pageRoute.FullUrlPath.Replace(RouteFormat, string.Empty);
        // if not found or there are other parameters in the route path not resolved.
        if (pathRoot.Contains('{'))
        {
            return null;
        }

        return url.Substring(pathRoot.Length - 1).Trim('/');
    }

    [GeneratedRegex(RegexLibary.SlugCaseInsensitive)]
    private static partial Regex SlugCaseInsensitiveRegex();
}
