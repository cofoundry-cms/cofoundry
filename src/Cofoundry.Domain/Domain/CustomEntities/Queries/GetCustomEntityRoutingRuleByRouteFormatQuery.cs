using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a custom entity routing rule with a specific route 
    /// format identifier. The routing rules are registered at
    /// startup and will have been checked to ensure there are no
    /// duplicate RouteFormat values.
    /// </summary>
    public class GetCustomEntityRoutingRuleByRouteFormatQuery : IQuery<ICustomEntityRoutingRule>
    {
        /// <summary>
        /// Finds a custom entity routing rule with a specific route 
        /// format identifier. The routing rules are registered at
        /// startup and will have been checked to ensure there are no
        /// duplicate RouteFormat values.
        /// </summary>
        /// <param name="routeFormat">
        /// The unique identifier for the routing rule to find. This is a
        /// a string representation of the route format e.g. "{Id}/{UrlSlug}"
        /// and will be unique.
        /// </param>
        public GetCustomEntityRoutingRuleByRouteFormatQuery(string routeFormat)
        {
            RouteFormat = routeFormat;
        }

        /// <summary>
        /// The unique identifier for the routing rule to find. This is a
        /// a string representation of the route format e.g. "{Id}/{UrlSlug}"
        /// and will be unique.
        /// </summary>
        public string RouteFormat { get; set; }
    }
}
