using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core
{
    /// <summary>
    /// Used to indicate whether the application should show the 
    /// developer exception page with full exception details or not.
    /// </summary>
    public enum DeveloperExceptionPageMode
    {
        /// <summary>
        /// Only shows the developer exception page when the environment name 
        /// is Microsoft.AspNetCore.Hosting.EnvironmentName.Development.
        /// </summary>
        DevelopmentOnly,

        /// <summary>
        /// Always shows the developer exception page.
        /// </summary>
        On,

        /// <summary>
        /// Never shows the developer exception page.
        /// </summary>
        Off
    }
}
