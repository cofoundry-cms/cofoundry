using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        IHtmlContent ToJson<T>(T value);
    }
}
