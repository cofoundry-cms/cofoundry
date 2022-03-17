using Cofoundry.Core.ResourceFiles;
using System.Reflection;

namespace Cofoundry.Domain;

/// <summary>
/// A file source for files embedded in an assembly.
/// </summary>
/// <inheritdoc/>
public class EmbeddedResourceFileSource : IFileSource
{
    /// <summary>
    /// Creates a new EmbeddedResourceFileSource instance.
    /// </summary>
    /// <param name="assembly">The assembly containing the embedded resource file.</param>
    /// <param name="path">
    /// <para>
    /// The path to the embedded resource without the file name. An embedded resource 
    /// path is usually formed from the default assembly namespace and the folder 
    /// path, delimited by '.' (period) e.g. "Cofoundry.Domain.ExampleParentFolder.ExampleChildFolder"
    /// </para>
    /// <para>
    /// Note that special characters such as spaces and dashes are replaced by underscores 
    /// in embedded resource names, however this conversion is done automatically for you
    /// during construction.
    /// </para>
    /// </param>
    /// <param name="fileName">
    /// <para>
    /// The file name including file extensions e.g. "myFile.jpg". 
    /// </para>
    /// </param>
    public EmbeddedResourceFileSource(
        Assembly assembly,
        string path,
        string fileName
        )
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));

        if (path == null) throw new ArgumentNullException(nameof(path));
        if (string.IsNullOrWhiteSpace(path)) throw new ArgumentEmptyException(nameof(path));

        if (fileName == null) throw new ArgumentNullException(nameof(fileName));
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentEmptyException(nameof(fileName));

        Assembly = assembly;
        FileName = fileName;
        FullPath = EmbeddedResourcePathFormatter.CleanPath(path) + "." + fileName;
    }

    /// <summary>
    /// Creates a new EmbeddedResourceFileSource instance.
    /// </summary>
    /// <param name="assembly">The assembly containing the embedded resource file.</param>
    /// <param name="fileName">The name of the file with an extention e.g. "MyFile.txt".</param>
    /// <param name="fullPath">The full embedded resource path e.g. "MyAssembly.MyFolder.MyFile.txt"</param>
    /// <param name="mimeType">The mime type of the file.</param>
    public EmbeddedResourceFileSource(
        Assembly assembly,
        string fileName,
        string fullPath,
        string mimeType
        )
    {
        if (assembly == null) throw new ArgumentNullException(nameof(assembly));

        if (fileName == null) throw new ArgumentNullException(nameof(fileName));
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentEmptyException(nameof(fileName));

        if (fullPath == null) throw new ArgumentNullException(nameof(fullPath));
        if (string.IsNullOrWhiteSpace(fullPath)) throw new ArgumentEmptyException(nameof(fullPath));

        Assembly = assembly;
        FileName = fileName;
        FullPath = fullPath;
        MimeType = mimeType;
    }

    /// <summary>
    /// The name of the file including file extension.
    /// </summary>
    public string FileName { get; private set; }

    /// <summary>
    /// Optional mime/content type associated with the file, if it is known. For
    /// embedded resources this is not often known.
    /// </summary>
    public string MimeType { get; private set; }

    /// <summary>
    /// The full embedded resource path including the namespace, folder 
    /// path and file name e.g. "MyAssembly.MyFolder.MyFile.txt".
    /// </summary>
    public string FullPath { get; private set; }

    /// <summary>
    /// The assembly containing the embedded resource file.
    /// </summary>
    public Assembly Assembly { get; private set; }

    /// <summary>
    /// Opens a new stream of the file contents. The callee is responsible for 
    /// disposing of the stream.
    /// </summary>
    public Task<Stream> OpenReadStreamAsync()
    {
        return Task.FromResult(Assembly.GetManifestResourceStream(FullPath));
    }
}
