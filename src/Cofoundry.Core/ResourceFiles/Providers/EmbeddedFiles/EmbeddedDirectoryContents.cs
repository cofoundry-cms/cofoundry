using Microsoft.Extensions.FileProviders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.ResourceFiles
{
    /// <summary>
    /// An IDirectoryContents that emulates a directory structure of
    /// embedded resources. Due to file path restrictions in the 
    /// embedded resource system the file path translation may not be
    /// accurate when certain special characters are used. Therefore file
    /// names with special characters should be avoided when embedding. The
    /// general rule is avoid anything that wouldn't be permitted in a class
    /// name. E.g. "jQuery.UI-2.1.1" could be renamed "jQuery_UI_2_1_1".
    /// </summary>
    public class EmbeddedDirectoryContents : IDirectoryContents
    {
        private readonly IReadOnlyCollection<IFileInfo> _entries;

        public EmbeddedDirectoryContents(string resourceDirectoryPath, IEnumerable<IFileInfo> allDirectoryFiles)
        {
            _entries = ParseDirectories(resourceDirectoryPath, allDirectoryFiles);
        }

        public bool Exists
        {
            get { return true; }
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        private IReadOnlyCollection<IFileInfo> ParseDirectories(string resourceDirectoryPath, IEnumerable<IFileInfo> allDirectoryFiles)
        {
            var directoryFiles = new List<IFileInfo>();
            var directories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var file in allDirectoryFiles)
            {
                var endPath = file.Name.Remove(0, resourceDirectoryPath.Length);
                var directoryName = GetDirectoryName(endPath);

                if (string.IsNullOrEmpty(directoryName))
                {
                    var fileName = GetFileName(file.Name);
                    var amendedFileInfoFile = new AmendedFileNameResourceFileInfo(fileName, file);
                    directoryFiles.Add(amendedFileInfoFile);
                }
                else if (!directories.Contains(directoryName))
                {
                    directories.Add(directoryName);
                    directoryFiles.Add(new EmbeddedResourceDirectoryInfo(directoryName, file.LastModified));
                }
            }

            return directoryFiles;
        }

        private static string GetFileName(string path)
        {
            var directoryParts = path.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            IEnumerable<string> fileNameParts = directoryParts;
            if (directoryParts.Length > 1)
            {
                fileNameParts = directoryParts.Skip(directoryParts.Length - 2);
            }

            var name = string.Join(".", fileNameParts);
            return name;
        }

        private static string GetDirectoryName(string path)
        {
            var directoryParts = path
                .Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (directoryParts.Length <= 2) return null;

            return directoryParts.First();
        }
    }
}
