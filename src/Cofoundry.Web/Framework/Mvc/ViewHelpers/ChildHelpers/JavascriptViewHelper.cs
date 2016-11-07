using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cofoundry.Web
{
    public class JavascriptViewHelper : IJavascriptViewHelper
    {
        public IHtmlString ToJson<T>(T value)
        {
            var valueJson = JsonConvert.SerializeObject(value);

            return new HtmlString(valueJson);
        }
    }
}
