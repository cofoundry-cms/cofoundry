using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        : CofoundryPageHelper<TModel> where TModel : IEditablePageViewModel
    {
        public CofoundryTemplatePageHelper(
            ViewContext viewContext, 
            TModel model
            )
            : base(model)
        {
            Template = new PageTemplateHelper<TModel>(viewContext, model);
        }

        /// <summary>
        /// Contains helper functionality relating to the page template
        /// such as section definitions.
        /// </summary>
        public IPageTemplateHelper<TModel> Template { get; set; }
    }
}
