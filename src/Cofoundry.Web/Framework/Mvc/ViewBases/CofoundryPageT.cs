using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public abstract class CofoundryPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        public override void InitHelpers()
        {
            base.InitHelpers();
            Cofoundry = new CofoundryPageHelper<TModel>(Html, this.Model);
        }

        public CofoundryPageHelper<TModel> Cofoundry { get; private set; }
    }

}
