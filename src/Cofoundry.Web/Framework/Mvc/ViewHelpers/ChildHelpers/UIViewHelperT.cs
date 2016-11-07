using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Helper for accessing Cofoundry UI controls.
    /// </summary>
    /// <remarks>
    /// Note that UI controls are provided using extension methods to 
    /// this helper in oder to seperate them neatly into their own code files.
    /// </remarks>
    public class UIViewHelper<T> : UIViewHelper, IUIViewHelper<T>
    {
        public UIViewHelper(HtmlHelper htmlHelper, T model)
            : base(htmlHelper)
        {
            Model = model;
        }

        public T Model { get; private set; }
    }
}
