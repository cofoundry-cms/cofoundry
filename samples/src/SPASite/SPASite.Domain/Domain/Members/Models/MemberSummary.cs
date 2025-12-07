namespace SPASite.Domain;

/// <summary>
/// A small model projection of a member.
/// </summary>
public class MemberSummary
{
    public required int UserId { get; set; }

    public required string DisplayName { get; set; }
}
