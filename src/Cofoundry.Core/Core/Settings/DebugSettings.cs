using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core.Configuration;

namespace Cofoundry.Core
{
    /// <summary>
    /// Settings that give finer grained control over debugging Cofoundry features.
    /// </summary>
    public class DebugSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// Disabled any robits.txt file and instead serves up a file that disallows 
        /// </summary>
        public bool DisableRobotsTxt { get; set; }

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
    }
}
