namespace Cofoundry.Domain;

/// <summary>
/// Cache for page block data, which is frequently requested to 
/// when rendering pages and does no change once the application
/// is running
/// </summary>
public interface IPageBlockTypeCache
{
    /// <summary>
    /// Gets all page block types if they are already cached, otherwise the 
    /// getter is invoked and the result is cached and returned
    /// </summary>
    /// <param name="getter">Function to invoke if the page block types aren't in the cache</param>
    Task<IReadOnlyCollection<PageBlockTypeSummary>> GetOrAddAsync(Func<Task<IReadOnlyCollection<PageBlockTypeSummary>>> getter);

    /// <summary>
    /// Gets all a collection of all PageBlockTypeFileLocation objects if they are already 
    /// cached, otherwise the getter is invoked and the result is cached and returned
    /// </summary>
    /// <param name="getter">Function to invoke if the page block types aren't in the cache</param>
    IReadOnlyDictionary<string, PageBlockTypeFileLocation> GetOrAddFileLocations(Func<IReadOnlyDictionary<string, PageBlockTypeFileLocation>> getter);

    /// <summary>
    /// Removes all block type data from the cache
    /// </summary>
    void Clear();
}
