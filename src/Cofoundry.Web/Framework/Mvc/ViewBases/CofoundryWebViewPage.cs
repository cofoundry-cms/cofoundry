using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public abstract class CofoundryWebViewPage : System.Web.Mvc.WebViewPage
    {
        public override void InitHelpers()
        {
            base.InitHelpers();
            Cofoundry = new CofoundryHelper(Html);
        }

        public CofoundryHelper Cofoundry { get; private set; }
    }
}
