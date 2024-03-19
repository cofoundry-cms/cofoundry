namespace Cofoundry.BasicTestSite;

public record BlogPostSummary()
{
    public required string Title { get; init; }

    public required string ShortDescription { get; init; }

    public required ImageAssetRenderDetails? ThumbnailImageAsset { get; init; }

    public required string? FullPath { get; init; }

    public required DateTime PostDate { get; init; }
}
