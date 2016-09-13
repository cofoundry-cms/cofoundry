using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Use this to mark up a property in a custom entity data model,
    /// this property will be extracted and added to the cached CustomEntityRoute
    /// object and therefore make the property available for routing operations
    /// without having to re-query the db. E.g. for a blog post custom entity you 
    /// could mark up a category Id and then use this in an ICustomEntityRoutingRule
    /// to create a /category/blog-post url route
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomEntityRouteDataAttribute : Attribute
    {
    }
}
