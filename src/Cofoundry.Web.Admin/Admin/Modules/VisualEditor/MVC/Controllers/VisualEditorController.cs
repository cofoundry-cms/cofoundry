using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorController : BaseAdminMvcController
    {
        public ActionResult Frame(VisualEditorFrameModel model)
        {
            var viewPath = ViewPathFormatter.View("VisualEditor", nameof(Frame));
            return View(viewPath, model);
        }
    }
}