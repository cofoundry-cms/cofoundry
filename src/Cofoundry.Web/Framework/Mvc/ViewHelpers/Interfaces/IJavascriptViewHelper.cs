using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cofoundry.Web
{
    public interface IJavascriptViewHelper
    {
        IHtmlString ToJson<T>(T value);
    }
}
