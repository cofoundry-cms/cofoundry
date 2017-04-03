using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public interface IPageActionRoutingStep
    {
        Task ExecuteAsync(Controller controller, PageActionRoutingState state);
    }
}
