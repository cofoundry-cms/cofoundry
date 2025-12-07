using Microsoft.AspNetCore.Html;

namespace SimpleSite;

public class AuthorDetails
{
    public required string Name { get; set; }

    public required IHtmlContent Biography { get; set; }

    public ImageAssetRenderDetails? ProfileImage { get; set; }
}
