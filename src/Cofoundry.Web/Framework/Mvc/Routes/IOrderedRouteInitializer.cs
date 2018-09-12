using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web
{
    /// <summary>
    /// Implement this interface to define a custom ordering value to your
    /// route registration, otherwise the default ordering value will
    /// be used.
    /// </summary>
    public interface IOrderedRouteRegistration : IRouteRegistration, IOrderedTask
    {
    }
}
