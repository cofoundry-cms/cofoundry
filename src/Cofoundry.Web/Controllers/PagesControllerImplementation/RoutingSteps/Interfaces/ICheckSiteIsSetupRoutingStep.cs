using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Checks if Cofoundry has been set up yet and if it hasn't, redirects the request
    /// to the setup screen.
    /// </summary>
    public interface ICheckSiteIsSetupRoutingStep : IPageActionRoutingStep
    {
    }
}
