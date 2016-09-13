using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Cofoundry.Core.AutoMapper
{
    /// <summary>
    /// A version of HtmlString with a default constructor which allows for Projection mapping.
    /// I think this issue https://github.com/AutoMapper/AutoMapper/issues/654 covers this and will
    /// hopefully get resolved soon.
    /// </summary>
    public class ProjectableHtmlString : IHtmlString
    {
        public ProjectableHtmlString()
        {
        }

        public ProjectableHtmlString(string value)
        {
            Value = value;
        }

        public string Value { get; set; }

        #region methods

        public string ToHtmlString()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value;
        }

        #endregion
    }
}
