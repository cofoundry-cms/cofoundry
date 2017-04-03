using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public abstract class CofoundryPage<TModel> : RazorPage<TModel>
    {
        private CofoundryPageHelper<TModel> _cofoundryPageHelper = null;
        
        public CofoundryPageHelper<TModel> Cofoundry
        {
            get
            {
                if (_cofoundryPageHelper == null && ViewContext != null)
                {
                    _cofoundryPageHelper = new CofoundryPageHelper<TModel>(Model);
                }

                return _cofoundryPageHelper;
            }
        }
    }

}
