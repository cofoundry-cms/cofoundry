using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public class UIViewHelper : IUIViewHelper
    {
        public UIViewHelper(
            HtmlHelper htmlHelper
            )
        {
            HtmlHelper = htmlHelper;
        }

        public HtmlHelper HtmlHelper { get; private set; }
    }
}
