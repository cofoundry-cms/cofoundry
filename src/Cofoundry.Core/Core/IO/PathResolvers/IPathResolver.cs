namespace Cofoundry.Core;

/// <summary>
/// Resolver for mapping application relative paths to absolute 
/// physical paths.
/// </summary>
public interface IPathResolver
{
    /// <summary>
    /// Returns the physical file path that corresponds to the specified relative 
    /// path. If the path value is <see langword="null"/> or empty then the application 
    /// root path is returned.
    /// </summary>
    /// <param name="path">virtual path to resolve</param>
    string MapPath(string? path);
}
