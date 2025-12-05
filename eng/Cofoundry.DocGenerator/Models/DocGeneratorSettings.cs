namespace Cofoundry.DocGenerator;

public class DocGeneratorSettings
{
    /// <summary>
    /// If <see langword="true"/> then docs will be output to the Azure storage
    /// account configured via <see cref="BlobStorageConnectionString"/>.
    /// </summary>
    public bool UseAzure { get; set; }

    /// <summary>
    /// The version to build, in the format "X.Y.Z".
    /// </summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>
    /// The full file system path to the root of the docs repo.
    /// </summary>
    public string SourcePath { get; set; } = string.Empty;

    /// <summary>
    /// When not using azure, the full file system path to the output directory.
    /// </summary>
    public string? OutputPath { get; set; }

    /// <summary>
    /// Whether to clear the destination directory or not. Typically
    /// you'd want to avoid this as you could potentially get missing file 
    /// errors on the website, having said that, as long as the cache has been
    /// built it shouldn't matter.
    /// </summary>
    public bool CleanDestination { get; set; }

    /// <summary>
    /// If publishing to azure blog storage, this is the connection string to use.
    /// </summary>
    public string? BlobStorageConnectionString { get; set; }

    /// <summary>
    /// A url to post to when the process has finished. Used to break
    /// the cache on the cofoundry.org website once the publish
    /// process has completed. If empty then this does not run.
    /// </summary>
    public string? OnCompleteWebHook { get; set; }
}
