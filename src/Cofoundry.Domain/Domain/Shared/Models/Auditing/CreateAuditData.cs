namespace Cofoundry.Domain;

/// <summary>
/// Simple audit data for entity creations.
/// </summary>
public class CreateAuditData
{
    /// <summary>
    /// The user that created the entity. Nullable to support
    /// the first user created in the system which has no creator.
    /// </summary>
    public UserMicroSummary? Creator { get; set; }

    /// <summary>
    /// The date the entity was created.
    /// </summary>
    public DateTime CreateDate { get; set; }

    /// <summary>
    /// A placeholder value to use for not-nullable values that you
    /// know will be initialized in later code. This value should not
    /// be used in data post-initialization.
    /// </summary>
    public static readonly CreateAuditData Uninitialized = new();
}
