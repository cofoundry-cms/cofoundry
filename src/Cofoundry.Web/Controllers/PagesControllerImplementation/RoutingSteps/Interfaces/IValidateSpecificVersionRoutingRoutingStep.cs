using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// If the request is for a specific page version, we validate that the user has permission to see 
    /// that version and that the version requested is valid. If it is not valid then the version
    /// parameters are discarded.
    /// </summary>
    public interface IValidateSpecificVersionRoutingRoutingStep : IPageActionRoutingStep
    {
    }
}
