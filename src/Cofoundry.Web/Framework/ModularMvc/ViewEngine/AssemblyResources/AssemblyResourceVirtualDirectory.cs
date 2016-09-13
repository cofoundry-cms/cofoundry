using Conditions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;

namespace Cofoundry.Web.ModularMvc
{
    /// <summary>
    /// A directory representation of embedded resources. Used to enable directory
    /// searching in the AssemblyResourceProvider.
    /// </summary>
    public class AssemblyResourceVirtualDirectory : VirtualDirectory
    {
        IEnumerable<AssemblyResourceVirtualDirectory> _childDirectories;
        IEnumerable<AssemblyResourceVirtualFile> _files;
        private readonly AssemblyResourceProviderSettings _assemblyResourceProviderSettings;
        private readonly IAssemblyResourcePhysicaFileRepository _assemblyPhysicalFileRepository;

        #region construction

        public AssemblyResourceVirtualDirectory(
            string vitualDirectory, 
            IEnumerable<AssemblyVirtualFileLocation> locations,
            IAssemblyResourcePhysicaFileRepository assemblyPhysicalFileRepository,
            AssemblyResourceProviderSettings assemblyResourceProviderSettings
            )
            : base(vitualDirectory)
        {
            Condition.Requires(vitualDirectory).IsNotNull();
            Condition.Requires(locations).IsNotNull();
            Condition.Requires(assemblyPhysicalFileRepository).IsNotNull();
            Condition.Requires(assemblyResourceProviderSettings).IsNotNull();

            _assemblyPhysicalFileRepository = assemblyPhysicalFileRepository;
            _assemblyResourceProviderSettings = assemblyResourceProviderSettings;

            ParseDirectories(vitualDirectory, locations);
        }

        private void ParseDirectories(string vitualDirectory, IEnumerable<AssemblyVirtualFileLocation> locations)
        {
            var directories = new Dictionary<string,List<AssemblyVirtualFileLocation>>();
            var files = new List<AssemblyResourceVirtualFile>();

            foreach (var location in locations)
            {
                var endPath = location.VirtualPath.Remove(0, vitualDirectory.Length);
                var path = Path.GetDirectoryName(endPath);

                if (string.IsNullOrEmpty(path))
                {
                    files.Add(new AssemblyResourceVirtualFile(location, _assemblyPhysicalFileRepository, _assemblyResourceProviderSettings));
                }
                else
                {
                    var directoryName = path
                        .Split(new char[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries)
                        .First();

                    if (directories.ContainsKey(directoryName))
                    {
                        directories[directoryName].Add(location);
                    }
                    else
                    {
                        directories.Add(directoryName, new List<AssemblyVirtualFileLocation>() { location });
                    }
                }
            }

            _files = files;
            _childDirectories = directories
                .Select(d => new AssemblyResourceVirtualDirectory(vitualDirectory + d.Key + "/", d.Value, _assemblyPhysicalFileRepository, _assemblyResourceProviderSettings));
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets a list of the files and subdirectories contained in this virtual directory.
        /// </summary>
        public override System.Collections.IEnumerable Children
        {
            get 
            {
                yield return _files;
                yield return _childDirectories; 
            } 
        }

        /// <summary>
        ///  Gets a list of all the subdirectories contained in this directory.
        /// </summary>
        public override System.Collections.IEnumerable Directories
        {
            get { return _childDirectories; } 
        }

        /// <summary>
        /// Gets a list of all files contained in this directory.
        /// </summary>
        public override System.Collections.IEnumerable Files
        {
            get
            {
                return _files;
            }
        }

        #endregion
    }
}
