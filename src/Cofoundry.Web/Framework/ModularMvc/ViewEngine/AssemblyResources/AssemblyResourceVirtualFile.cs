using Conditions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// Represents a resource file embedded in an assembly
    /// </summary>
    public class AssemblyResourceVirtualFile : VirtualFile
    {
        private readonly AssemblyVirtualFileLocation _location;
        private readonly IAssemblyResourcePhysicaFileRepository _assemblyPhysicalFileService;
        private readonly AssemblyResourceProviderSettings _assemblyResourceProviderSettings;

        public AssemblyResourceVirtualFile(
            AssemblyVirtualFileLocation location,
            IAssemblyResourcePhysicaFileRepository assemblyPhysicalFileService,
            AssemblyResourceProviderSettings assemblyResourceProviderSettings
            )
            : base(location.VirtualPath)
        {
            Condition.Requires(location).IsNotNull();
            Condition.Requires(location.VirtualPath).IsNotNullOrEmpty();

            _location = location;
            _assemblyPhysicalFileService = assemblyPhysicalFileService;
            _assemblyResourceProviderSettings = assemblyResourceProviderSettings;
        }

        public override Stream Open()
        {
            if (_assemblyResourceProviderSettings.BypassEmbeddedContent)
            {
                // If we're debugging and want to get the file directly
                // This prevents having to re-compile when working with cshtml/js/css files
                return _assemblyPhysicalFileService.Open(_location);
            }
            else
            {
                // Get the resource embedded in the assembly
                var stream = _location.Assembly.GetManifestResourceStream(_location.Path);
                return stream;
            }
        }
    }
}