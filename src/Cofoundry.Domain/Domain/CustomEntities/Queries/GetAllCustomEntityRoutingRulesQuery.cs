using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Query to return a collection of all ICustomEntityRoutingRule implementations
    /// registered with the DI system. This query checks the validity of the 
    /// collection before returning them.
    /// </summary>
    public class GetAllCustomEntityRoutingRulesQuery : IQuery<ICollection<ICustomEntityRoutingRule>>
    {
    }
}
