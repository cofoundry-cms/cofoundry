namespace Cofoundry.Domain;

/// <summary>
/// Model representing an empty or uninitialized instance of <see cref="IPageBlockTypeDisplayModel"/>,
/// used to initialize non-nullable projection properties before the true value 
/// is mapped.
/// </summary>
public record UninitializedPageBlockTypeDisplayModel : IPageBlockTypeDisplayModel
{
    /// <summary>
    /// An empty or uninitialized instance of <see cref="IPageBlockTypeDisplayModel"/>,
    /// used to initialize non-nullable projection properties before the true value 
    /// is mapped.
    /// </summary>
    public static readonly UninitializedPageBlockTypeDisplayModel Instance = new();
}
