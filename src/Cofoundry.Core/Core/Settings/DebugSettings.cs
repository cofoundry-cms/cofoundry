using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Cofoundry.Core
{
    /// <summary>
    /// Settings that give finer grained control over debugging Cofoundry features.
    /// </summary>
    public partial class DebugSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// Disables the dynamic robots.txt file and instead serves up a file that 
        /// disallows all.
        /// </summary>
        public bool DisableRobotsTxt { get; set; }

        /// <summary>
        /// Used to indicate whether the application should show the 
        /// developer exception page with full exception details or not.
        /// By default this is set to Cofoundry.Core.DeveloperExceptionPageMode.DevelopmentOnly.
        /// </summary>
        public DeveloperExceptionPageMode DeveloperExceptionPageMode { get; set; } = DeveloperExceptionPageMode.DevelopmentOnly;

        /// <summary>
        /// By default Cofoundry will try and load minified css/js files, but
        /// this can be overriden for debugging purposes and an uncompressed
        /// version will try and be located first.
        /// </summary>
        public bool UseUncompressedResources { get; set; }

        /// <summary>
        /// Use this to bypass resources embedded in assemblies and instead load them straight from the 
        /// file system. This is intended to be used when debugging the Cofoundry project to avoid having to re-start 
        /// the project when embedded resources have been updated. False by default.
        /// </summary>
        public bool BypassEmbeddedContent { get; set; }

        /// <summary>
        /// If bypassing embedded content, MapPath will be used to determine the folder root unless this override
        /// is specified. The assembly name is added to the path to make the folder root of the project with the resource in.
        /// </summary>
        public string EmbeddedContentPhysicalPathRootOverride { get; set; }

        /// <summary>
        /// USe to determine if we should show the developer exception page,
        /// taking the current environment into consideration.
        /// </summary>
        /// <param name="env">The current hosting environment.</param>
        /// <returns>True if we can show the developer exception page; otherwise false.</returns>
        public bool CanShowDeveloperExceptionPage(IWebHostEnvironment env)
        {
            return DeveloperExceptionPageMode == DeveloperExceptionPageMode.On
                || (DeveloperExceptionPageMode == DeveloperExceptionPageMode.DevelopmentOnly && env.IsDevelopment());
        }
    }
}
