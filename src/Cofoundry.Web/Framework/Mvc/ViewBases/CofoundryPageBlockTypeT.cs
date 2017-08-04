using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public abstract class CofoundryPageBlockType<TModel> 
        : RazorPage<TModel> //where TModel : IPageBlockTypeDisplayModel
    {
        private CofoundryPageBlockTypeHelper<TModel> _cofoundryPageHelper = null;

        public CofoundryPageBlockTypeHelper<TModel> Cofoundry
        {
            get
            {
                if (_cofoundryPageHelper == null && ViewContext != null)
                {
                    _cofoundryPageHelper = new CofoundryPageBlockTypeHelper<TModel>(ViewContext, Model);
                }

                return _cofoundryPageHelper;
            }
        }
    }

}
