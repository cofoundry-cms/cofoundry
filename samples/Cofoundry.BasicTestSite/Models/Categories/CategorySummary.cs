namespace Cofoundry.BasicTestSite;

public record CategorySummary
{
    public required int CategoryId { get; init; }

    public required string Title { get; init; }

    public required string? ShortDescription { get; init; }
}
