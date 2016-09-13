using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Cofoundry.Core.AutoMapper
{
    /// <summary>
    /// Automapper type converter for converting strings to HtmlStrings.
    /// </summary>
    public class StringToHtmlStringConverter : ITypeConverter<string, HtmlString>
    {
        public HtmlString Convert(string source, HtmlString destination, ResolutionContext context)
        {
            return new HtmlString(source);
        }
    }
}
