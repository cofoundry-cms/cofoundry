using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public abstract class CofoundryPage : System.Web.Mvc.WebViewPage
    {
        public override void InitHelpers()
        {
            base.InitHelpers();
            Cofoundry = new CofoundryPageHelper(Html);
        }

        public CofoundryPageHelper Cofoundry { get; private set; }
    }
}
