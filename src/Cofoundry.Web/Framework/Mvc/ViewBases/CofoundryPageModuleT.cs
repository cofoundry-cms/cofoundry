using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public abstract class CofoundryPageModule<TModel> 
        : System.Web.Mvc.WebViewPage<TModel> where TModel : IPageModuleDisplayModel
    {
        public override void InitHelpers()
        {
            base.InitHelpers();
            Cofoundry = new CofoundryPageModuleHelper<TModel>(Html, this.Model);
        }

        public CofoundryPageModuleHelper<TModel> Cofoundry { get; private set; }
    }

}
