using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public interface IUIViewHelper
    {
        HtmlHelper HtmlHelper { get; }
    }
}
