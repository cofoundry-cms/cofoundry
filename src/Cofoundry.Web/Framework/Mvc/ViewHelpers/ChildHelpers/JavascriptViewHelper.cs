using Microsoft.AspNetCore.Html;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class JavascriptViewHelper : IJavascriptViewHelper
    {
        public IHtmlContent ToJson<T>(T value)
        {
            var valueJson = JsonConvert.SerializeObject(value);

            return new HtmlString(valueJson);
        }
    }
}
