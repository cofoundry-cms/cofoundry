namespace SimpleSite;

public class BlogPostSummary
{
    public required string Title { get; set; }

    public required string ShortDescription { get; set; }

    public required string? AuthorName { get; set; }

    public ImageAssetRenderDetails? ThumbnailImageAsset { get; set; }

    public required string FullPath { get; set; }

    public required DateTime? PostDate { get; set; }
}
