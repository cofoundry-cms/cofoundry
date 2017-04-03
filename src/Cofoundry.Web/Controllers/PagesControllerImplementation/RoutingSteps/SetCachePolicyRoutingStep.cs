using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Here we set a cache policy for the result before it goes out.
    /// </summary>
    /// <remarks>
    /// Not much is happenening here yet, this code has just been carried forward from an earlier version.
    /// </remarks>
    public class SetCachePolicyRoutingStep : ISetCachePolicyRoutingStep
    {
        public Task ExecuteAsync(Controller controller, PageActionRoutingState state)
        {
            // TODO: Come up with a better caching policy, but for now
            // don't cache if we're logged into Cofoundry Admin
            if (state.UserContext.IsCofoundryUser())
            {
                controller.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                controller.Response.Cache.SetNoStore();
            }

            return Task.FromResult(true);
        }
    }
}
