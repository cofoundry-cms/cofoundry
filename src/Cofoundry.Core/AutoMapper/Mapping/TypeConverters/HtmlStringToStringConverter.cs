using AutoMapper;
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Cofoundry.Core.AutoMapper
{
    /// <summary>
    /// Automapper type converter for converting HtmlStrings to strings.
    /// </summary>
    public class HtmlStringToStringConverter : ITypeConverter<HtmlString, string>
    {
        public string Convert(HtmlString source, string destination, ResolutionContext context)
        {
            return System.Convert.ToString(source);
        }
    }
}
