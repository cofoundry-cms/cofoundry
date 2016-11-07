using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Factory that enables ICustomEntityTemplateSectionTagBuilderFactory implementation to be swapped out.
    /// </summary>
    /// <remarks>
    /// The factory is required because the HtmlHelper cannot be injected
    /// </remarks>
    public class CustomEntityTemplateSectionTagBuilderFactory : ICustomEntityTemplateSectionTagBuilderFactory
    {
        private readonly IModuleRenderer _moduleRenderer;

        public CustomEntityTemplateSectionTagBuilderFactory(
            IModuleRenderer moduleRenderer)
        {
            _moduleRenderer = moduleRenderer;
        }

        public ICustomEntityTemplateSectionTagBuilder<TModel> Create<TModel>(
            HtmlHelper htmlHelper,
            CustomEntityDetailsPageViewModel<TModel> customEntityViewModel, 
            string sectionName
            )
            where TModel : ICustomEntityDetailsDisplayViewModel
        {
            return new CustomEntityTemplateSectionTagBuilder<TModel>(_moduleRenderer, htmlHelper, customEntityViewModel, sectionName);
        }
    }
}
