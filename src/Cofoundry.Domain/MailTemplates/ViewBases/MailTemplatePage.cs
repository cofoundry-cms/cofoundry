using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public abstract class MailTemplatePage<TModel> : RazorPage<TModel>
    {
        private CofoundryMailTemplatePageHelper<TModel> _cofoundryPageHelper = null;

        public CofoundryMailTemplatePageHelper<TModel> Cofoundry
        {
            get
            {
                if (_cofoundryPageHelper == null && ViewContext != null)
                {
                    _cofoundryPageHelper = new CofoundryMailTemplatePageHelper<TModel>(ViewContext, Model);
                }

                return _cofoundryPageHelper;
            }
        }
    }
}
