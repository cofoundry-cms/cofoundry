using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public interface INotFoundViewHelper
    {
        ActionResult GetView();
    }
}