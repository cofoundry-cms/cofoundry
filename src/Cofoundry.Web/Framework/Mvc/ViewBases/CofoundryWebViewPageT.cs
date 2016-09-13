using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public abstract class CofoundryWebViewPage<T> : System.Web.Mvc.WebViewPage<T>
    {
        public override void InitHelpers()
        {
            base.InitHelpers();
            Cofoundry = new CofoundryHelper<T>(Html, this.Model);
        }

        public CofoundryHelper<T> Cofoundry { get; private set; }
    }

}
