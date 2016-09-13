using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Cofoundry.Web
{
    public interface IUIViewHelper<T> : IUIViewHelper
    {
        T Model { get; }
    }
}
