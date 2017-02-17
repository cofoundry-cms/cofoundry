using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public interface IPageViewModelMapper
    {
        PageViewModel Map(PageRenderDetails page, VisualEditorMode siteViewerMode);
        IEditablePageViewModel MapCustomEntityModel(
            Type displayModelType,
            PageRenderDetails page,
            CustomEntityRenderDetails customEntityRenderDetails,
            VisualEditorMode siteViewerMode);
        T Map<T>(PageRenderDetails page, VisualEditorMode siteViewerMode) where T : IEditablePageViewModel, IPageRoutableViewModel, new();
    }
}
