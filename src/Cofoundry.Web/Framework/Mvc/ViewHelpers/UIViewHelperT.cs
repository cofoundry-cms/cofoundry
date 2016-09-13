using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
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
