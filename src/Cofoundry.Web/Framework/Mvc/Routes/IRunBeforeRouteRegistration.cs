using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    /// <summary>
    /// Implement this interface to define routes that should be
    /// injected into the MVC route collection during the startup 
    /// process. The RunBefore property can be used to indicate that
    /// this registration should be run before other registrations
    /// are executed.
    /// </summary>
    public interface IRunBeforeRouteRegistration : IRouteRegistration, IRunBeforeTask
    {
    }
}