namespace Cofoundry.Domain;

/// <summary>
/// Abstraction of a file in the process of being uploaded. In ASP.NET terms
/// this is an abstraction of IFormFile but has broader scope and can be used 
/// to represent files uploaded to Cofoundry by other mechanisms.
/// </summary>
public interface IFileSource
{
    /// <summary>
    /// The name of the file including file extension (if available). For some
    /// implementations the file name is specified by the client and so cannot 
    /// be trusted.
    /// </summary>
    string FileName { get; }

    /// <summary>
    /// Optional mime/content type associated with the file, if it is known. For some 
    /// implementations the mime type is specified by the client and is not to be 
    /// trusted.
    /// </summary>
    string MimeType { get; }

    /// <summary>
    /// Opens a stream of the file contents. The callee is responsible for disposing 
    /// of the stream.
    /// </summary>
    Task<Stream> OpenReadStreamAsync();
}
