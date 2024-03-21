using System.Text.RegularExpressions;

namespace Cofoundry.Domain;

/// <summary>
/// A routing rule that uses the CustomEntityId and UrlSlug properties
/// to match the custom entity. When routing the request will be 
/// redirected if the UrlSlug does not match exactly; 
/// </summary>
public partial class IdAndUrlSlugCustomEntityRoutingRule : ICustomEntityRoutingRule
{
    /// <inheritdoc/>
    public string RouteFormat => "{Id}/{UrlSlug}";

    /// <inheritdoc/>
    public int Priority => (int)RoutingRulePriority.Normal;

    /// <inheritdoc/>
    public bool RequiresUniqueUrlSlug => false;

    /// <inheritdoc/>
    public bool MatchesRule(string url, PageRoute pageRoute)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(url);
        ArgumentNullException.ThrowIfNull(pageRoute);

        var routingPart = GetRoutingPart(url, pageRoute);
        if (string.IsNullOrEmpty(routingPart))
        {
            return false;
        }

        var match = RouteRegex().Match(routingPart);

        if (!match.Success)
        {
            return false;
        }

        var isMatch = IntParser.ParseOrDefault(match.Groups[1].Value) > 0;

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

        var routingPart = GetRoutingPart(url, pageRoute);

        if (string.IsNullOrEmpty(routingPart))
        {
            throw new InvalidOperationException($"{nameof(routingPart)} is not expected to be null when {nameof(MatchesRule)} is true. Url: {url}, pageRoute: {pageRoute.FullUrlPath}");
        }

        var match = RouteRegex().Match(routingPart);

        var query = new GetCustomEntityRouteByPathQuery
        {
            CustomEntityDefinitionCode = pageRoute.CustomEntityDefinitionCode,
            CustomEntityId = Convert.ToInt32(match.Groups[1].Value, CultureInfo.InvariantCulture)
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

        return pageRoute.FullUrlPath
            .Replace("{Id}", entityRoute.CustomEntityId.ToString(CultureInfo.InvariantCulture))
            .Replace("{UrlSlug}", entityRoute.UrlSlug);
    }

    /// <summary>
    /// Extracts the custom entity routing part of the path from 
    /// a <paramref name="url"/> e.g. url "/my-path/123" with Pageroute 
    /// "/my-path/{id}" will return "123".
    /// </summary>
    private string? GetRoutingPart(string url, PageRoute pageRoute)
    {
        if (!pageRoute.FullUrlPath.Contains(RouteFormat))
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
    /// <summary>
    /// id must be an integer, slug is optional
    /// </summary>
    [GeneratedRegex(@"^(\d+)(?:\/([a-zA-Z0-9-_]+))?$")]
    private static partial Regex RouteRegex();
}
