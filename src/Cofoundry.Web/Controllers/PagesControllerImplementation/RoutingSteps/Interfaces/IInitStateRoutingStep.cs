using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Initialises the key parameters of the PageActionRoutingState
    /// object e.g. the UserContext and VisualEditorMode properties
    /// </summary>
    public interface IInitStateRoutingStep : IPageActionRoutingStep
    {
    }
}
