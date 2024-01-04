namespace Cofoundry.Domain;

/// <summary>
/// Model representing an empty or uninitialized instance of <see cref="ICustomEntityDataModel"/>,
/// used to initialize non-nullable projection properties before the true value 
/// is mapped.
/// </summary>
public record UninitializedCustomEntityDataModel : ICustomEntityDataModel
{
    /// <summary>
    /// An empty or uninitialized instance of <see cref="ICustomEntityDataModel"/>,
    /// used to initialize non-nullable projection properties before the true value 
    /// is mapped.
    /// </summary>
    public static readonly UninitializedCustomEntityDataModel Instance = new();
}
