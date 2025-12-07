namespace SPASite.Domain;

/// <summary>
/// The difference between the CatDetails and CatSummary model
/// is small, but it illustrates how the CQS lets us tailor 
/// models returned from queries to fit different situations.
/// </summary>
public class CatDetails
{
    public required int CatId { get; set; }

    public required string Name { get; set; }

    public required string? Description { get; set; }

    public required int TotalLikes { get; set; }

    public required Breed? Breed { get; set; }

    public required IReadOnlyCollection<Feature> Features { get; set; }

    public required IReadOnlyCollection<ImageAssetRenderDetails> Images { get; set; }
}
