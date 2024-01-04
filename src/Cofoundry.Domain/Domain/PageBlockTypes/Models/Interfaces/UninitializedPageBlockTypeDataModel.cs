namespace Cofoundry.Domain;

/// <summary>
/// Model representing an empty or uninitialized instance of <see cref="IPageBlockTypeDataModel"/>,
/// used to initialize non-nullable projection properties before the true value 
/// is mapped.
/// </summary>
public record UninitializedPageBlockTypeDataModel : IPageBlockTypeDataModel
{
    /// <summary>
    /// An empty or uninitialized instance of <see cref="IPageBlockTypeDataModel"/>,
    /// used to initialize non-nullable projection properties before the true value 
    /// is mapped.
    /// </summary>
    public static readonly UninitializedPageBlockTypeDataModel Instance = new();
}
