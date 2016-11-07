using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cofoundry.Web
{
    /// <summary>
    /// Helper for working with javascript from .net code
    /// </summary>
    public interface IJavascriptViewHelper
    {
        /// <summary>
        /// Serializes the specified object into json using the default json serializer
        /// </summary>
        /// <param name="value">Object to serialize</param>
        IHtmlString ToJson<T>(T value);
    }
}
