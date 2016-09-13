using Conditions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// Used to get the physical file represented by an AssemblyVirtualFile. Used
    /// in dev when the BypassEmbeddedContent setting is set to true to load the 
    /// files from disk instead of from the assembly. Allows file modification without re-compiling
    /// all the time.
    /// </summary>
    public class AssemblyResourcePhysicaFileRepository : IAssemblyResourcePhysicaFileRepository
    {
        private readonly IPathResolver _pathResolver;
        private readonly AssemblyResourceProviderSettings _assemblyResourceProviderSettings;

        public AssemblyResourcePhysicaFileRepository(
            IPathResolver pathResolver,
            AssemblyResourceProviderSettings assemblyResourceProviderSettings
            )
        {
            _pathResolver = pathResolver;
            _assemblyResourceProviderSettings = assemblyResourceProviderSettings;

        }

        public Stream Open(AssemblyVirtualFileLocation location)
        {
            Condition.Requires(location).IsNotNull();
            Condition.Requires(location.VirtualPath).IsNotNullOrEmpty();

            var physicalPath = GetPhysicalFilePath(location);
            return ReadFile(physicalPath);
        }

        private Stream ReadFile(string filePath)
        {
            var stream = new MemoryStream();
            using (var reader = File.OpenRead(filePath))
            {
                reader.CopyTo(stream);
            }
            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        public string GetPhysicalFilePath(AssemblyVirtualFileLocation location)
        {
            // Work out the file path, rewriting it to use the correct project based on the assembly name
            string path = null;
            string virtualPathWithoutTilda = location.VirtualPath.TrimStart('~');
            var virtualPath = location.VirtualPath;

            if (!string.IsNullOrWhiteSpace(_assemblyResourceProviderSettings.EmbeddedContentPhysicalPathRootOverride))
            {
                path = Path.Combine(_assemblyResourceProviderSettings.EmbeddedContentPhysicalPathRootOverride.Trim('/'), virtualPath.Replace('\\', '/').Trim('/'));
            }
            else
            {
                path = _pathResolver.MapPath(virtualPath);
            }

            var basePath = path.Substring(0, path.Length - virtualPathWithoutTilda.Length);
            var filePath = path.Substring(path.Length - virtualPathWithoutTilda.Length);
            var pathParts = basePath.Split(Path.DirectorySeparatorChar);
            // Assumes the folder name is the same as the assembly name
            var allPathParts = pathParts
                .Take(pathParts.Length - 1)
                .Concat(new string[] { location.Assembly.GetName().Name })
                .ToList();

            var newPath = string.Join(Path.DirectorySeparatorChar.ToString(), allPathParts);
            var completePath = newPath + filePath;

            return completePath;
        }

        public DateTime GetModifiedDate(AssemblyVirtualFileLocation location)
        {
            var physicalPath = GetPhysicalFilePath(location);
            return File.GetLastWriteTimeUtc(physicalPath);
        }
    }
}
