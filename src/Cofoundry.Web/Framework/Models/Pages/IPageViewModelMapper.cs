using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public interface IPageViewModelMapper
    {
        PageViewModel Map(PageRenderDetails page, SiteViewerMode siteViewerMode);
        IEditablePageViewModel MapCustomEntityModel(
            Type displayModelType,
            PageRenderDetails page,
            CustomEntityRenderDetails customEntityRenderDetails,
            SiteViewerMode siteViewerMode);
        T Map<T>(PageRenderDetails page, SiteViewerMode siteViewerMode) where T : IEditablePageViewModel, IPageRoutableViewModel, new();
    }
}
