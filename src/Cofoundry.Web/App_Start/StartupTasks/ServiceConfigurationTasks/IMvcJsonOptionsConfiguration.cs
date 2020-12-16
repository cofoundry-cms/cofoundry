using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    /// <summary>
    /// Implement this interface to extend the MvcJsonOptions configuration in a modular 
    /// fashion. Implementations can make use of dependency injection, however this is
    /// built using a temporary service collection that will be disposed of after 
    /// configuration is complete.
    /// </summary>
    public interface IMvcJsonOptionsConfiguration
    {
        /// <summary>
        /// Performs additional option configuration. 
        /// </summary>
        /// <param name="options">The options to perform configuration on.</param>
        void Configure(MvcNewtonsoftJsonOptions options);
    }
}
