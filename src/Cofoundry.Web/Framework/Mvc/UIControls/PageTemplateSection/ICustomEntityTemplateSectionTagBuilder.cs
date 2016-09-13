using System;
using System.Web;

namespace Cofoundry.Web
{
    public interface ICustomEntityTemplateSectionTagBuilder<TModel> : IHtmlString
        where TModel : ICustomEntityDetailsDisplayViewModel
    {
        ICustomEntityTemplateSectionTagBuilder<TModel> AllowMultipleModules();
        ICustomEntityTemplateSectionTagBuilder<TModel> EmptyContentMinHeight(int minHeight);
        ICustomEntityTemplateSectionTagBuilder<TModel> WrapWithTag(string tagName, object htmlAttributes = null);
    }
}
