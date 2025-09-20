using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ImageSharpExample.Pages;

public class IndexModel : PageModel
{
    private readonly IContentRepository _contentRepository;

    public IndexModel(
        IContentRepository contentRepository
        )
    {
        ImageAssetId = 1;
        Width = 600;
        Height = 400;
        _contentRepository = contentRepository;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (ImageAssetId.HasValue)
        {
            ImageAsset = await _contentRepository
                .ImageAssets()
                .GetById(ImageAssetId.Value)
                .AsRenderDetails()
                .ExecuteAsync();
        }

        return Page();
    }

    public ImageAssetRenderDetails? ImageAsset { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? ImageAssetId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Width { get; set; }

    [BindProperty(SupportsGet = true)]
    public int Height { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool UseBackgroundColor { get; set; }

    [BindProperty(SupportsGet = true)]
    public ImageAnchorLocation? AnchorLocation { get; set; }

    [BindProperty(SupportsGet = true)]
    public ImageScaleMode? Scale { get; set; }

    public IImageResizeSettings CreateResizeSettings(ImageFitMode imageFitMode)
    {
        var settings = new ImageResizeSettings()
        {
            Height = Height,
            Mode = imageFitMode,
            Width = Width
        };

        if (UseBackgroundColor)
        {
            settings.BackgroundColor = "663399";
        }

        if (Scale.HasValue)
        {
            settings.Scale = Scale.Value;
        }

        if (AnchorLocation.HasValue)
        {
            settings.Anchor = AnchorLocation.Value;
        }

        return settings;
    }
}
