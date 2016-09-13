using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Class for rendering views to strings
    /// </summary>
    public class RazorViewRenderer
    {
        #region constructor

        private readonly ControllerContext _controllerContext;

        public RazorViewRenderer(
            ControllerContext controllerContext
            )
        {
            _controllerContext = controllerContext;
        }

        #endregion

        #region public methods

        public string RenderView(string viewName)
        {
            viewName = GetViewName(viewName);
            var viewResult = ViewEngines.Engines.FindView(_controllerContext, viewName, null);

            return Render(viewResult);
        }

        public string RenderView<T>(string viewName, T model)
        {
            viewName = GetViewName(viewName);
            var viewResult = ViewEngines.Engines.FindView(_controllerContext, viewName, null);

            return Render(viewResult, model);
        }

        public string RenderPartialView(string viewName)
        {
            viewName = GetViewName(viewName);
            var viewResult = ViewEngines.Engines.FindPartialView(_controllerContext, viewName);

            return Render(viewResult);
        }

        public string RenderPartialView<T>(string viewName, T model)
        {
            viewName = GetViewName(viewName);
            var viewResult = ViewEngines.Engines.FindPartialView(_controllerContext, viewName);

            return Render(viewResult, model);
        }

        #endregion

        #region helpers

        private string Render<T>(ViewEngineResult viewResult, T model)
        {
            var viewData = new ViewDataDictionary<T>(model);
            return RenderViewData(viewResult, viewData);
        }

        private string Render(ViewEngineResult viewResult)
        {
            var viewData = new ViewDataDictionary();
            return RenderViewData(viewResult, viewData);
        }

        private string RenderViewData(ViewEngineResult viewResult, ViewDataDictionary viewData)
        {
            var tempData = new TempDataDictionary();

            using (var sw = new StringWriter())
            {
                var viewContext = new ViewContext(_controllerContext, viewResult.View, viewData, tempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString().Trim(@"\r\n".ToCharArray());
            }
        }

        private string GetViewName(string viewName)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = _controllerContext.RouteData.GetRequiredString("action");
            }

            return viewName;
        }

        #endregion
    }
}
