using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public abstract class CofoundryPage : RazorPage<dynamic>
    {
        private CofoundryPageHelper _cofoundryPageHelper = null;

        public CofoundryPageHelper Cofoundry
        {
            get
            {
                if (_cofoundryPageHelper == null && ViewContext != null)
                {
                    _cofoundryPageHelper = new CofoundryPageHelper(ViewContext);
                }

                return _cofoundryPageHelper;
            }
        }
    }
}
