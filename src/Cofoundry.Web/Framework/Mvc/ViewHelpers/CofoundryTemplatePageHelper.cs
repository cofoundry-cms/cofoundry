using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Main helper for Cofoundry functionality on PageTemplate 
    /// views. Typically accessed via @Cofoundry, this keeps 
    /// all cofoundry functionality under one helper to avoid 
    /// polluting the global namespace.
    /// </summary>
    public class CofoundryTemplatePageHelper<TModel> 
        : CofoundryPageHelper<TModel>, ICofoundryTemplateHelper<TModel> 
        where TModel : IEditablePageViewModel
    {
        public CofoundryTemplatePageHelper(
            IContentRouteLibrary contentRouteLibrary,
            IStaticFileViewHelper staticFileViewHelper,
            ISettingsViewHelper settings,
            ICurrentUserViewHelper currentUser,
            IJavascriptViewHelper js,
            IHtmlSanitizerHelper sanitizer,
            ICofoundryHtmlHelper html,
            IPageTemplateHelper<TModel> pageTemplateHelper

            )
            : base(
                contentRouteLibrary,
                staticFileViewHelper,
                settings,
                currentUser,
                js,
                sanitizer,
                html
                )
        {
            Template = pageTemplateHelper;
        }

        public override void Contextualize(ViewContext viewContext)
        {
            base.Contextualize(viewContext);

            if (Template is IViewContextAware viewContextAware)
            {
                viewContextAware.Contextualize(viewContext);
            }
        }

        /// <summary>
        /// Contains helper functionality relating to the page template
        /// such as region definitions.
        /// </summary>
        public IPageTemplateHelper<TModel> Template { get; set; }
    }
}
