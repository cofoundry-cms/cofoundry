using System;
using System.Web.Mvc;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public interface IModuleRenderer
    {
        string RenderModule(ControllerBase controller, IEditablePageViewModel pageViewModel, IEntityVersionPageModuleRenderDetails moduleViewModel);
        string RenderModule(HtmlHelper htmlHelper, IEditablePageViewModel pageViewModel, IEntityVersionPageModuleRenderDetails moduleViewModel);
        string RenderPlaceholderModule(int? minHeight = null);
    }
}
