using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public abstract class CofoundryTemplatePage<TModel> : RazorPage<TModel> 
        where TModel : IEditablePageViewModel
    {
        private CofoundryTemplatePageHelper<TModel> _cofoundryPageHelper = null;

        public CofoundryTemplatePageHelper<TModel> Cofoundry
        {
            get
            {
                if (_cofoundryPageHelper == null && ViewContext != null)
                {
                    _cofoundryPageHelper = new CofoundryTemplatePageHelper<TModel>(ViewContext, Model);
                }

                return _cofoundryPageHelper;
            }
        }
    }
}
