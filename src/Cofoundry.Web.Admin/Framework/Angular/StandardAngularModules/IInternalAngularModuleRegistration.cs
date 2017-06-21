using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    /// <summary>
    /// Implement this to register a module as an 'internal' admin module i.e. for use 
    /// in this assembly only.
    /// </summary>
    internal interface IInternalAngularModuleRegistration : IStandardAngularModuleRegistration
    {
    }
}