namespace SPASite.Domain;

/// <summary>
/// The difference between the CatDetails and CatSummary model
/// is small, but it illustrates how the CQS lets us tailor 
/// models returned from queries to fit different situations.
/// </summary>
public class CatSummary
{
    public required int CatId { get; set; }

    public required string Name { get; set; }

    public required string? Description { get; set; }

    public required int TotalLikes { get; set; }

    public ImageAssetRenderDetails? MainImage { get; set; }
}
