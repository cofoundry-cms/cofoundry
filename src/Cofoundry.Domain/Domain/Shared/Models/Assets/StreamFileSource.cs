namespace Cofoundry.Domain;

/// <summary>
/// A basic <see cref="IFileSource"/> that gives you complete control 
/// over the internal file stream.
/// </summary>
/// <inheritdoc/>
public class StreamFileSource : IFileSource
{
    private readonly Func<Task<Stream>> _streamFactory;

    /// <param name="fileName">
    /// The name of the file including file extension (if available).
    /// </param>
    /// <param name="streamFactory">
    /// A function that returns a data stream of the file. When invoked
    /// the callee will be responsible for disposing of the stream, however
    /// the factory function may not always be called.
    /// </param>
    public StreamFileSource(string fileName, Func<Task<Stream>> streamFactory)
        : this(fileName, null, streamFactory)
    {
    }

    /// <param name="fileName">
    /// The name of the file including file extension (if available).
    /// </param>
    /// <param name="streamFactory">
    /// A function that returns a data stream of the file. When invoked
    /// the callee will be responsible for disposing of the stream, however
    /// the factory function may not always be called.
    /// </param>
    public StreamFileSource(string fileName, Func<Stream> streamFactory)
        : this(fileName, null, streamFactory)
    {
    }

    /// <param name="fileName">
    /// The name of the file including file extension (if available).
    /// </param>
    /// <param name="mimeType">
    /// Optional mime/content type associated with the file, if it is known.
    /// </param>
    /// <param name="streamFactory">
    /// A function that returns a data stream of the file. When invoked
    /// the callee will be responsible for disposing of the stream, however
    /// the factory function may not always be called.
    /// </param>
    public StreamFileSource(string fileName, string mimeType, Func<Stream> streamFactory)
        : this(fileName, mimeType, () => Task.FromResult(streamFactory()))
    {
        ArgumentNullException.ThrowIfNull(streamFactory);
    }

    /// <param name="fileName">
    /// The name of the file including file extension (if available).
    /// </param>
    /// <param name="mimeType">
    /// Optional mime/content type associated with the file, if it is known.
    /// </param>
    /// <param name="streamFactory">
    /// A function that returns a data stream of the file. When invoked
    /// the callee will be responsible for disposing of the stream, however
    /// the factory function may not always be called.
    /// </param>
    public StreamFileSource(string fileName, string mimeType, Func<Task<Stream>> streamFactory)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(fileName);
        ArgumentNullException.ThrowIfNull(streamFactory);

        FileName = fileName;
        MimeType = mimeType;
        _streamFactory = streamFactory;
    }

    public string FileName { get; private set; }

    public string MimeType { get; private set; }

    public Task<Stream> OpenReadStreamAsync()
    {
        return _streamFactory();
    }
}
