namespace Cofoundry.Plugins.SiteMap;

/// <summary>
/// Factory for creating concrete <see cref="ISiteMapBuilder"/> objects.
/// </summary>
public interface ISiteMapBuilderFactory
{
    /// <summary>
    /// Creates a new instance of an <see cref="ISiteMapBuilder"/>.
    /// </summary>
    ISiteMapBuilder Create();
}
