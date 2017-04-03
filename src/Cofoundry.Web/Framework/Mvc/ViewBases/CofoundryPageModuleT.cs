using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public abstract class CofoundryPageModule<TModel> 
        : RazorPage<TModel> where TModel : IPageModuleDisplayModel
    {
        private CofoundryPageModuleHelper<TModel> _cofoundryPageHelper = null;

        public CofoundryPageModuleHelper<TModel> Cofoundry
        {
            get
            {
                if (_cofoundryPageHelper == null && ViewContext != null)
                {
                    _cofoundryPageHelper = new CofoundryPageModuleHelper<TModel>(Model);
                }

                return _cofoundryPageHelper;
            }
        }
    }

}
