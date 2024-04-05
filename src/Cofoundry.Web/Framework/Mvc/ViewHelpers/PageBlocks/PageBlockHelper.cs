namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="IPageBlockHelper{TViewModel}"/>.
/// </summary>
public class PageBlockHelper<TViewModel> : IPageBlockHelper<TViewModel>
{
    /// <inheritdoc/>
    public IPageBlockHelper<TViewModel> UseDisplayName(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (name.Length > 50)
        {
            throw new Exception("Block display name cannot be longer than 50 characters.");
        }

        // nothing is rendered here, this is just used as a convention for adding template meta data
        return this;
    }

    /// <inheritdoc/>
    public IPageBlockHelper<TViewModel> UseDescription(string description)
    {
        ArgumentNullException.ThrowIfNull(description);
        // nothing is rendered here, this is just used as a convention for adding template meta data
        return this;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        // Overridden to make sure no output is rendered into a view when
        // we use method chaining
        return string.Empty;
    }
}
