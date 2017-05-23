using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    [Area(RouteConstants.AdminAreaName)]
    [Route(RouteConstants.AdminAreaPrefix + "/" + VisualEditorRouteLibrary.RoutePrefix)]
    public class VisualEditorController : Controller
    {
        #region routes

        [Route("frame")]
        public ActionResult Frame(VisualEditorFrameModel model)
        {
            var viewPath = ViewPathFormatter.View("VisualEditor", nameof(Frame));
            return View(viewPath, model);
        }

        #endregion
    }
}