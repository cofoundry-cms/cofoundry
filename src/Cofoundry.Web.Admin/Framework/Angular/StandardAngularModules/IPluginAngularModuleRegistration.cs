using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Implement this to register a module as a 'plugin' admin module, which will 
    /// prevent path clashed with core/client modules.
    /// </summary>
    public interface IPluginAngularModuleRegistration : IStandardAngularModuleRegistration
    {
    }
}