using System;
using System.Web;

namespace Cofoundry.Web
{
    public interface IPageTemplateSectionTagBuilder : IHtmlString
    {
        IPageTemplateSectionTagBuilder AllowMultipleModules();
        IPageTemplateSectionTagBuilder EmptyContentMinHeight(int minHeight);
        IPageTemplateSectionTagBuilder WrapWithTag(string tagName, object htmlAttributes = null);
    }
}
