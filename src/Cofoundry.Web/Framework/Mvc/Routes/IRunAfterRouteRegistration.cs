using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Implement this interface to define routes that should be
    /// injected into the MVC route collection during the startup 
    /// process. The RunAfter property can be used to indicate that
    /// this registration is dependent on another registration being 
    /// executed first.
    /// </summary>
    public interface IRunAfterRouteRegistration : IRouteRegistration, IRunAfterTask
    {
    }
}