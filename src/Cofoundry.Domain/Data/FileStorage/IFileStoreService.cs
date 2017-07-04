using System;
using System.IO;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// File system abstraction service
    /// </summary>
    public interface IFileStoreService
    {
        /// <summary>
        /// Determins if the specified file exists in the container
        /// </summary>
        /// <param name="containerName">The name of the container to look for the file</param>
        /// <param name="fileName">Name of the file to look for</param>
        /// <returns>True if the file exists; otherwise false</returns>
        Task<bool> ExistsAsync(string containerName, string fileName);

        /// <summary>
        /// Gets the specified file as a Stream. 
        /// </summary>
        /// <param name="containerName">The name of the container to look for the file</param>
        /// <param name="fileName">The name of the file to get</param>
        /// <returns>Stream reference to the file.</returns>
        Task<Stream> GetAsync(string containerName, string fileName);

        /// <summary>
        /// Creates a new file, throwing an exception if a file already exists with the same filename
        /// </summary>
        Task CreateAsync(string containerName, string fileName, Stream stream);

        /// <summary>
        /// Saves a file, creating a new file or overwriting a file if it already exists.
        /// </summary>
        Task CreateOrReplaceAsync(string containerName, string fileName, Stream stream);

        /// <summary>
        /// Creates a new file if it doesn't exist already, otherwise the existing file is left in place.
        /// </summary>
        Task CreateIfNotExistsAsync(string containerName, string fileName, Stream stream);

        /// <summary>
        /// Deletes a file from the container if it exists.
        /// </summary>
        /// <param name="containerName">The name of the container containing the file to delete</param>
        /// <param name="fileName">Name of the file to delete</param>
        Task DeleteAsync(string containerName, string fileName);

        /// <summary>
        /// Deletes a directory including all files and sub-directories
        /// </summary>
        /// <param name="containerName">The name of the container containing the directory to delete</param>
        /// <param name="directoryName">The name of the directory to delete</param>
        Task DeleteDirectoryAsync(string containerName, string directoryName);

        /// <summary>
        /// Clears a directory deleting all files and sub-directories but not the directory itself
        /// </summary>
        /// <param name="containerName">The name of the container containing the directory to delete</param>
        /// <param name="directoryName">The name of the directory to delete</param>
        Task ClearDirectoryAsync(string containerName, string directoryName);

        /// <summary>
        /// Deletes all files in the container
        /// </summary>
        /// <param name="containerName">Name of the container to clear.</param>
        Task ClearContainerAsync(string containerName);
    }
}
