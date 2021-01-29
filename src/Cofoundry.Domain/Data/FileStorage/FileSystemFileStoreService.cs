using Cofoundry.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data.Internal
{
    /// <summary>
    /// File storage abstraction using the file system
    /// </summary>
    public class FileSystemFileStoreService : IFileStoreService
    {
        #region constructor

        private readonly Lazy<string> _fileRoot;

        private readonly FileSystemFileStorageSettings _fileSystemFileStorageSettings;
        private readonly IPathResolver _pathResolver;

        public FileSystemFileStoreService(
            FileSystemFileStorageSettings fileSystemFileStorageSettings,
            IPathResolver pathResolver
            )
        {
            _fileSystemFileStorageSettings = fileSystemFileStorageSettings;
            _pathResolver = pathResolver;
            _fileRoot = new Lazy<string>(SetFilePath);
        }

        private string SetFilePath()
        {
            string fileRoot = _pathResolver.MapPath(_fileSystemFileStorageSettings.FileRoot);

            if (!Directory.Exists(fileRoot))
            {
                Directory.CreateDirectory(fileRoot);
            }

            return fileRoot;
        }
            
        #endregion

        #region IFileStoreService implementation

        /// <summary>
        /// Determins if the specified file exists in the container
        /// </summary>
        /// <param name="containerName">The name of the container to look for the file</param>
        /// <param name="fileName">Name of the file to look for</param>
        /// <returns>True if the file exists; otherwise false</returns>
        public Task<bool> ExistsAsync(string containerName, string fileName)
        {
            var exists = File.Exists(Path.Combine(_fileRoot.Value, containerName, fileName));
            return Task.FromResult(exists);
        }

        /// <summary>
        /// Gets the specified file as a Stream. 
        /// </summary>
        /// <param name="containerName">The name of the container to look for the file</param>
        /// <param name="fileName">The name of the file to get</param>
        /// <returns>Stream reference to the file.</returns>
        public Task<Stream> GetAsync(string containerName, string fileName)
        {
            var path = Path.Combine(_fileRoot.Value, containerName, fileName);
            Stream fileStream = null;

            if (File.Exists(path))
            {
                fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }

            return Task.FromResult(fileStream);
        }

        /// <summary>
        /// Creates a new file, throwing an exception if a file already exists with the same filename
        /// </summary>
        public Task CreateAsync(string containerName, string fileName, System.IO.Stream stream)
        {
            return CreateFileAsync(containerName, fileName, stream, FileMode.CreateNew);
        }

        /// <summary>
        /// Saves a file, creating a new file or overwriting a file if it already exists.
        /// </summary>
        public Task CreateOrReplaceAsync(string containerName, string fileName, Stream stream)
        {
            return CreateFileAsync(containerName, fileName, stream, FileMode.Create);
        }

        /// <summary>
        /// Creates a new file if it doesn't exist already, otherwise the existing file is left in place.
        /// </summary>
        public Task CreateIfNotExistsAsync(string containerName, string fileName, System.IO.Stream stream)
        {
            var path = Path.Combine(_fileRoot.Value, containerName, fileName);
            if (File.Exists(path)) return Task.CompletedTask;

            return CreateFileAsync(containerName, fileName, stream, FileMode.CreateNew);
        }

        public Task DeleteAsync(string containerName, string fileName)
        {
            var path = Path.Combine(_fileRoot.Value, containerName, fileName);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Deletes a directory including all files and sub-directories
        /// </summary>
        /// <param name="containerName">The name of the container containing the directory to delete</param>
        /// <param name="directoryName">The name of the directory to delete</param>
        public Task DeleteDirectoryAsync(string containerName, string directoryName)
        {
            var dir = Path.Combine(_fileRoot.Value, containerName, directoryName);
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Clears a directory deleting all files and sub-directories but not the directory itself
        /// </summary>
        /// <param name="containerName">The name of the container containing the directory to delete</param>
        /// <param name="directoryName">The name of the directory to delete</param>
        public Task ClearDirectoryAsync(string containerName, string directoryName)
        {
            var dir = Path.Combine(_fileRoot.Value, containerName, directoryName);
            if (Directory.Exists(dir))
            {
                var directory = new DirectoryInfo(dir);
                foreach (var file in directory.GetFiles())
                {
                    if (file.IsReadOnly) file.IsReadOnly = false;
                    file.Delete();
                }
                foreach (var childDirectory in directory.GetDirectories())
                {
                    childDirectory.Delete(true);
                }
            }

            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Deletes all files in the container
        /// </summary>
        /// <param name="containerName">Name of the container to clear.</param>
        public Task ClearContainerAsync(string containerName)
        {
            return ClearDirectoryAsync(containerName, string.Empty);
        }

        #endregion

        #region private helpers

        private async Task CreateFileAsync(string containerName, string fileName, Stream stream, FileMode fileMode)
        {
            var path = Path.Combine(_fileRoot.Value, containerName, fileName);
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            using (var fs = new FileStream(path, fileMode))
            {
                stream.Position = 0;
                await stream.CopyToAsync(fs);
            }
        }

        #endregion
    }
}
