using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Custom entity routing rules respresent the dynamic routing 
    /// rules used to work out which custom  entity should be displayed 
    /// in a custom entity details page. E.g. a rule with a format of
    /// "{Id}/{UrlSlug}" uses those parameters to identify a custom entity.
    /// </summary>
    public interface ICustomEntityRoutingRule
    {
        /// <summary>
        /// A string representation of the route format e.g. "{Id}/{UrlSlug}". Used as a display value
        /// but also as the unique identifier for the rule, so it shouldn't clash with any other routing rule.
        /// </summary>
        string RouteFormat { get; }

        /// <summary>
        /// Sets a priority over which rules should be run in case more than one is used in the
        /// same page directory. Custom integer values can be used but use RoutingRulePriority whenever possible
        /// to avoid hardcoding to a specific value.
        /// </summary>
        int Priority { get; }

        /// <summary>
        /// Indicates whether this rule can only be used with custom entities with a unique url slug.
        /// </summary>
        bool RequiresUniqueUrlSlug { get; }

        /// <summary>
        /// Indicates whether the specified url matches this routing rule.
        /// </summary>
        /// <param name="url">The url to test</param>
        /// <param name="pageRoute">The page route already matched to this url.</param>
        bool MatchesRule(string url, PageRoute pageRoute);

        /// <summary>
        /// Returns a query that can be used to look up the CustomEntityRoute relating 
        /// to the matched entity. Throws an exception if the MatchesRule returns false, so
        /// check this before calling this method.
        /// </summary>
        /// <param name="url">The url to parse custom entity key data from</param>
        /// <param name="pageRoute">The page route matched to the url</param>
        /// <returns>An IQuery object that can used to query for the CustomEntityRoute</returns>
        IQuery<CustomEntityRoute> ExtractRoutingQuery(string url, PageRoute pageRoute);

        /// <summary>
        /// Transforms the routing specified routing information into a full, relative url.
        /// </summary>
        /// <param name="pageRoute">The matched page route for the url</param>
        /// <param name="entityRoute">The matched custom entity route for the url</param>
        /// <returns>Full, relative url</returns>
        string MakeUrl(PageRoute pageRoute, CustomEntityRoute entityRoute);
    }
}
