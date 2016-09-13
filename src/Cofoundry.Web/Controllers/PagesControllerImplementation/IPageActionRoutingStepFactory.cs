using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// A factory that creates a collection of ordered IPageActionRoutingStep to
    /// be executed in order during the PageController's Page Action. This determines
    /// the routing of dynamic page content in the site.
    /// </summary>
    public interface IPageActionRoutingStepFactory
    {
        IEnumerable<IPageActionRoutingStep> Create();
    }
}
