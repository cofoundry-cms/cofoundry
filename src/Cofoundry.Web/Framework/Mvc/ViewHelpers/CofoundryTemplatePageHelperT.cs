using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Main helper for Cofoundry functionality on PageTemplate 
    /// views. Typically accessed via @Cofoundry, this keeps 
    /// all cofoundry functionality under one helper to avoid 
    /// poluting the global namespace.
    /// </summary>
    public class CofoundryTemplatePageHelper<TModel> 
        : CofoundryPageHelper<TModel> where TModel : IEditablePageViewModel
    {
        public CofoundryTemplatePageHelper(HtmlHelper htmlHelper, TModel model)
            : base(htmlHelper, model)
        {
            Template = new PageTemplateHelper<TModel>(htmlHelper, model);
        }

        /// <summary>
        /// Contains helper functionality relating to the page template
        /// such as section definitions.
        /// </summary>
        public IPageTemplateHelper<TModel> Template { get; set; }
    }
}
