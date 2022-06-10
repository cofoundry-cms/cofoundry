using System.Text.RegularExpressions;

namespace Cofoundry.Domain;

/// <summary>
/// A routing rule that uses the CustomEntityId and UrlSlug properties
/// to match the custom entity. When routing the request will be 
/// redirected if the UrlSlug does not match exactly; 
/// </summary>
public class IdAndUrlSlugCustomEntityRoutingRule : ICustomEntityRoutingRule
{
    /// <summary>
    /// id must be an integer, slug is optional
    /// </summary>
    private const string ROUTE_REGEX = @"^(\d+)(?:\/([a-zA-Z0-9-_]+))?$";

    /// <summary>
    /// A string representation of the route format e.g.  "{Id}/{UrlSlug}". Used as a display value
    /// but also as the unique identifier for the rule, so it shouldn't clash with any other routing rule.
    /// </summary>
    public string RouteFormat
    {
        get { return "{Id}/{UrlSlug}"; }
    }

    /// <summary>
    /// Sets a priority over which rules should be run in case more than one is used in the
    /// same page directory. Custom integer values can be used but use RoutingRulePriority whenever possible
    /// to avoid hardcoding to a specific value.
    /// </summary>
    public int Priority
    {
        get { return (int)RoutingRulePriority.Normal; }
    }

    /// <summary>
    /// Indicates whether this rule can only be used with custom entities with a unique 
    /// url slug, indicated by the ForceUrlSlugUniqueness setting on the 
    /// <see cref="ICustomEntityDefinition"/> implementation.
    /// </summary>
    public bool RequiresUniqueUrlSlug
    {
        get { return false; }
    }

    /// <summary>
    /// Indicates whether the specified url matches this routing rule.
    /// </summary>
    /// <param name="url">The url to test</param>
    /// <param name="pageRoute">The page route already matched to this url.</param>
    public bool MatchesRule(string url, PageRoute pageRoute)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(url);
        ArgumentNullException.ThrowIfNull(pageRoute);

        var routingPart = GetRoutingPart(url, pageRoute);
        if (string.IsNullOrEmpty(routingPart)) return false;

        var match = Regex.Match(routingPart, ROUTE_REGEX);

        if (!match.Success) return false;

        var isMatch = IntParser.ParseOrDefault(match.Groups[1].Value) > 0;

        return isMatch;
    }

    /// <summary>
    /// Returns a query that can be used to look up the CustomEntityRoute relating 
    /// to the matched entity. Throws an exception if the MatchesRule returns false, so
    /// check this before calling this method.
    /// </summary>
    /// <param name="url">The url to parse custom entity key data from</param>
    /// <param name="pageRoute">The page route matched to the url</param>
    /// <returns>An IQuery object that can used to query for the CustomEntityRoute</returns>
    public IQuery<CustomEntityRoute> ExtractRoutingQuery(string url, PageRoute pageRoute)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(url);
        ArgumentNullException.ThrowIfNull(pageRoute);

        if (!MatchesRule(url, pageRoute))
        {
            throw new ArgumentException(nameof(url) + $" does not match the specified page route. {nameof(ExtractRoutingQuery)} can only be called after a successful page route match.", nameof(url));
        }

        var routingPart = GetRoutingPart(url, pageRoute);

        var match = Regex.Match(routingPart, ROUTE_REGEX);

        var query = new GetCustomEntityRouteByPathQuery();
        query.CustomEntityDefinitionCode = pageRoute.CustomEntityDefinitionCode;
        query.CustomEntityId = Convert.ToInt32(match.Groups[1].Value);

        if (pageRoute.Locale != null)
        {
            query.LocaleId = pageRoute.Locale.LocaleId;
        }

        return query;
    }

    /// <summary>
    /// Transforms the routing specified routing information into a full, relative url.
    /// </summary>
    /// <param name="pageRoute">The matched page route for the url</param>
    /// <param name="entityRoute">The matched custom entity route for the url</param>
    /// <returns>Full, relative url</returns>
    public string MakeUrl(PageRoute pageRoute, CustomEntityRoute entityRoute)
    {
        ArgumentNullException.ThrowIfNull(pageRoute);
        ArgumentNullException.ThrowIfNull(entityRoute);

        return pageRoute.FullUrlPath
            .Replace("{Id}", entityRoute.CustomEntityId.ToString())
            .Replace("{UrlSlug}", entityRoute.UrlSlug);
    }

    /// <summary>
    /// Extracts the custom entity routing part of the path from 
    /// a <paramref name="url"/> e.g. url "/my-path/123" with Pageroute 
    /// "/my-path/{id}" will return "123".
    /// </summary>
    private string GetRoutingPart(string url, PageRoute pageRoute)
    {
        if (pageRoute.FullUrlPath.IndexOf(RouteFormat) == -1) return null;

        var pathRoot = pageRoute.FullUrlPath.Replace(RouteFormat, string.Empty);
        // if not found or there are other parameters in the route path not resolved.
        if (pathRoot.Contains('{')) return null;

        return url.Substring(pathRoot.Length - 1).Trim('/');
    }
}
