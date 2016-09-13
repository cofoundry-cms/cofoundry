using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Collections;
using System.Web.Caching;
using System.IO;
using System.Web.Optimization;
using Cofoundry.Core.EmbeddedResources;
using Cofoundry.Core;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// An assembly resource provider that gets all the views out of a DLL. To register your
    /// assembly and allow resources to be extracted from it create a concrete instance of IAssemblyResourceRegistration
    /// in the assembly.
    /// </summary>
    /// <remarks>
    /// Modified version of code found here:
    /// https://github.com/imranbaloch/ASP.NET-Optimization-Framework-and-Embeded-Resource/tree/master/SampleHelper
    /// </remarks>
    public class AssemblyResourceProvider : System.Web.Hosting.VirtualPathProvider
    {
        #region constructor

        private readonly Dictionary<string, AssemblyVirtualFileLocation> _resourceFiles = new Dictionary<string,AssemblyVirtualFileLocation>();
        private readonly IAssemblyResourcePhysicaFileRepository _assemblyPhysicalFileRepository;
        private readonly AssemblyResourceProviderSettings _assemblyResourceProviderSettings;

        public AssemblyResourceProvider(
            IAssemblyResourceRegistration[] registrations,
            IAssemblyResourcePhysicaFileRepository assemblyPhysicalFileRepository,
            AssemblyResourceProviderSettings assemblyResourceProviderSettings
            )
        {
            _assemblyPhysicalFileRepository = assemblyPhysicalFileRepository;
            _assemblyResourceProviderSettings = assemblyResourceProviderSettings;

            InitResources(registrations);
        }

        #endregion

        #region overrides

        public override bool FileExists(string virtualPath)
        {
            var exists = IsAssemblyResourcePath(virtualPath) || base.FileExists(virtualPath);

            return exists;
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            var location = GetAssemblyFileLocation(virtualPath);

            if (location != null)
            {
                return new AssemblyResourceVirtualFile(location, _assemblyPhysicalFileRepository, _assemblyResourceProviderSettings);
            }
            else
            {
                return base.GetFile(virtualPath);
            }
        }
        
        public override bool DirectoryExists(string virtualDir)
        {
            var locations = GetAssemblyFileLocations(virtualDir);
            var exists = (locations != null && locations.Any()) || base.DirectoryExists(virtualDir);

            return exists;
        }

        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            var locations = GetAssemblyFileLocations(virtualDir);

            if (locations != null && locations.Any())
            {
                return new AssemblyResourceVirtualDirectory(virtualDir, locations, _assemblyPhysicalFileRepository, _assemblyResourceProviderSettings);
            }
            else
            {
                return base.GetDirectory(virtualDir);
            }
        }

        public override CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (IsBundlePath(virtualPath)) return null;

            var location = GetAssemblyFileLocation(virtualPath);
            var isAppResource = location != null;
            var bypassEmbeddedContent = _assemblyResourceProviderSettings.BypassEmbeddedContent;

            // Ignore cache dependencies on bundles and embedded resources
            if (isAppResource && bypassEmbeddedContent)
            {
                var physicalFilePath = _assemblyPhysicalFileRepository.GetPhysicalFilePath(location);
                return new CacheDependency(physicalFilePath, utcStart);
            }
            else if (isAppResource)
            {
                return null;
            }

            return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        /// <remarks>
        /// Modified to suppress file caching when debugging:
        /// http://stackoverflow.com/a/16924284/716689
        /// </remarks>
        public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        {
            var bypassEmbeddedContent = _assemblyResourceProviderSettings.BypassEmbeddedContent;
            var location = GetAssemblyFileLocation(virtualPath);
            var isAppResource = location != null;

            if (isAppResource && bypassEmbeddedContent)
            {
                var hash = virtualPath + _assemblyPhysicalFileRepository.GetModifiedDate(location).Ticks;
                return hash;
            }

            return base.GetFileHash(virtualPath, virtualPathDependencies);
        }

        #endregion

        #region helpers

        private bool IsBundlePath(string path)
        {
            var isBundlePath =  BundleTable.Bundles.Any(b => b.Path == path);
            return isBundlePath;
        }

        private void InitResources(IAssemblyResourceRegistration[] registrations)
        {
            foreach (var registration in registrations)
            {
                var assembly = registration.GetType().Assembly;

                var resources = assembly
                    .GetManifestResourceNames()
                    .Select(r => new AssemblyVirtualFileLocation()
                    {
                        Assembly = assembly,
                        Path = r,
                        PathWithoutAssemblyName = r.Replace(assembly.GetName().Name + ".", "")
                    });

                foreach (var resource in resources)
                {
                    // To provide the ability to have view files in different assemblies we unfortunately have
                    // to make sure their paths are unique in all assemblies.
                    if (_resourceFiles.ContainsKey(resource.PathWithoutAssemblyName))
                    {
                        var msg = string.Format("Ducpliate view path detected, please rename the path to make it unique. {0} in assembly {1} conflicts with a file in assembly {3}",
                            resource.PathWithoutAssemblyName,
                            assembly.GetName().Name,
                            _resourceFiles[resource.PathWithoutAssemblyName].Assembly.GetName().Name
                            );
                        throw new ApplicationException(msg);
                    }

                    // make virtual file name
                    resource.VirtualPath = MakeVirtualFileName(resource);

                    _resourceFiles.Add(resource.PathWithoutAssemblyName.ToLowerInvariant(), resource);
                }
            }
        }

        private bool IsAssemblyResourcePath(string virtualPath)
        {
            var location = GetAssemblyFileLocation(virtualPath);
            return location != null;
        }

        private AssemblyVirtualFileLocation GetAssemblyFileLocation(string virtualPath)
        {
            // Iterator for display templates contains generic type with brackets which breaks
            if (virtualPath.Contains("<") || virtualPath.Contains(">")) return null;

            string checkPath = VirtualPathUtility.ToAppRelative(virtualPath);
            if (base.FileExists(virtualPath)) return null;

            var key = GetResourceKey(virtualPath).ToLowerInvariant();

            if (_resourceFiles.ContainsKey(key))
            {
                // Verify virtual path, because sometimes we cannot tell if underscores should be interpreted as dashes in filenames.
                _resourceFiles[key].VirtualPath = checkPath;
                return _resourceFiles[key];
            }

            return null;
        }

        private AssemblyVirtualFileLocation[] GetAssemblyFileLocations(string virtualPath)
        {
            // Iterator for display templates contains generic type with brackets which breaks
            if (virtualPath.Contains("<") || virtualPath.Contains(">")) return null;

            string checkPath = VirtualPathUtility.ToAppRelative(virtualPath);
            if (base.DirectoryExists(virtualPath)) return null;

            var dir = GetResourceKey(virtualPath).ToLowerInvariant();

            var files = _resourceFiles
                .Where(d => d.Key.StartsWith(dir))
                .Select(d => d.Value)
                .ToArray();

            return files;
        }

        private string GetResourceKey(string webString)
        {
            var fileName = Path.GetFileName(webString);
            var fileNameIndex = webString.LastIndexOf(fileName);
            var basePath = webString.Substring(0, fileNameIndex);

            string assemblyString = basePath
                .Replace("-", "_")
                .Replace("~/", "")
                .Replace("/", ".")
                .TrimStart(new char[] { '.' })
                + fileName;

            return assemblyString.ToLowerInvariant();
        }

        private string MakeVirtualFileName(AssemblyVirtualFileLocation resource)
        {
            var resourcePath = resource.PathWithoutAssemblyName;
            var fileExt = Path.GetExtension(resourcePath);
            var fileExtIndex = resourcePath.LastIndexOf(fileExt);
            var basePath = resourcePath.Substring(0, fileExtIndex);

            var virtualDir = "~/" + basePath
                .Replace("_", "-")
                .Replace(".", "/")
                + fileExt;

            return virtualDir;
        }

        #endregion
    }
}