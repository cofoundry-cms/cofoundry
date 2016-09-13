using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    public interface IPageRoutableViewModel
    {
        PageRoutingHelper PageRoutingHelper { get; set; }
    }
}