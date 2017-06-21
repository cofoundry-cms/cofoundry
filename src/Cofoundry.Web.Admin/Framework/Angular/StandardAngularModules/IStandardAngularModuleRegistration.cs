using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Implement this to register a module as a 'standard' admin module to cut down
    /// on boilerplate code. See StandardAngularModuleRegistrationBootstrapper for
    /// information on what actually gets registered. If you need more control register 
    /// the module using IAdminModuleRegistration instead.
    /// </summary>
    public interface IStandardAngularModuleRegistration
    {
        AdminModule GetModule();

        string RoutePrefix { get; }
    }
}