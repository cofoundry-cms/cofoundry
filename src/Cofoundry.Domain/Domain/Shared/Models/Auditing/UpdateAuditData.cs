namespace Cofoundry.Domain;

/// <summary>
/// Simple audit data for updatable entities.
/// </summary>
public class UpdateAuditData : CreateAuditData
{
    /// <summary>
    /// The user that last updated the entity. When first created
    /// the updater will be assigned to the user that created the 
    /// entity, ensuring this property always has a value.
    /// </summary>
    public UserMicroSummary Updater { get; set; } = UserMicroSummary.Uninitialized;

    /// <summary>
    /// The date the entity was last updated. When first created
    /// this value will be set tot he creation date, ensuring this
    /// property always has a value.
    /// </summary>
    public DateTime UpdateDate { get; set; }

    /// <summary>
    /// A placeholder value to use for not-nullable values that you
    /// know will be initialized in later code. This value should not
    /// be used in data post-initialization.
    /// </summary>
    public static new UpdateAuditData Uninitialized = new();
}
