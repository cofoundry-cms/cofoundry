namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="IVisualEditorStateService"/>.
/// The default service is used when the admin package is not installed
/// and simply returns an empty result.
/// </summary>
public class DefaultVisualEditorStateService : IVisualEditorStateService
{
    private VisualEditorState? _visualEditorStateCache = null;

    /// <inheritdoc/>
    public Task<VisualEditorState> GetCurrentAsync()
    {
        _visualEditorStateCache ??= new VisualEditorState();

        return Task.FromResult(_visualEditorStateCache);
    }
}
