namespace Cofoundry.Web;

public interface IVisualEditorStateCache
{
    /// <summary>
    /// Gets the cache value or <see langword="null"/> if it has not been set.
    /// </summary>
    VisualEditorState? Get();

    /// <summary>
    /// Sets the cache value.
    /// </summary>
    void Set(VisualEditorState data);
}
