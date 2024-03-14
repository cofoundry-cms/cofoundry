namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="IPageBlockHelper<>"/>.
/// </summary>
public class PageBlockHelper<TModel> : IPageBlockHelper<TModel>
{
    /// <inheritdoc/>
    public IPageBlockHelper<TModel> UseDisplayName(string name)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(name);

        if (name.Length > 50)
        {
            throw new Exception("Block display name cannot be longer than 50 characters.");
        }

        // nothing is rendered here, this is just used as a convention for adding template meta data
        return this;
    }

    /// <inheritdoc/>
    public IPageBlockHelper<TModel> UseDescription(string description)
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
