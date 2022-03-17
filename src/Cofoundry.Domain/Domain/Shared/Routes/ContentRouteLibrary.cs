using Cofoundry.Core.Web;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class ContentRouteLibrary : IContentRouteLibrary
{
    private readonly IDocumentAssetRouteLibrary _documentAssetRouteLibrary;
    private readonly IImageAssetRouteLibrary _imageAssetRouteLibrary;
    private readonly IPageRouteLibrary _pageRouteLibrary;
    private readonly ISiteUrlResolver _siteUriResolver;

    public ContentRouteLibrary(
        IPageRouteLibrary pageRouteLibrary,
        IImageAssetRouteLibrary imageAssetRouteLibrary,
        IDocumentAssetRouteLibrary documentAssetRouteLibrary,
        ISiteUrlResolver siteUriResolver
        )
    {
        _pageRouteLibrary = pageRouteLibrary;
        _imageAssetRouteLibrary = imageAssetRouteLibrary;
        _documentAssetRouteLibrary = documentAssetRouteLibrary;
        _siteUriResolver = siteUriResolver;
    }

    #region pages

    public Task<string> PageAsync(int? pageId)
    {
        return _pageRouteLibrary.PageAsync(pageId);
    }

    public string Page(IPageRoute route)
    {
        return _pageRouteLibrary.Page(route);
    }

    public string Page(ICustomEntityRoutable customEntity)
    {
        return _pageRouteLibrary.Page(customEntity);
    }

    #endregion

    #region images

    public Task<string> ImageAssetAsync(int? imageAssetId, IImageResizeSettings settings = null)
    {
        return _imageAssetRouteLibrary.ImageAssetAsync(imageAssetId, settings);
    }

    public Task<string> ImageAssetAsync(int? imageAssetId, int? width, int? height = null)
    {
        return _imageAssetRouteLibrary.ImageAssetAsync(imageAssetId, width, height);
    }

    public string ImageAsset(IImageAssetRenderable image, IImageResizeSettings settings = null)
    {
        return _imageAssetRouteLibrary.ImageAsset(image, settings);
    }

    public string ImageAsset(IImageAssetRenderable image, int? width, int? height = null)
    {
        return _imageAssetRouteLibrary.ImageAsset(image, width, height);
    }

    #endregion

    #region documents

    public Task<string> DocumentAssetAsync(int? documentAssetId)
    {
        return _documentAssetRouteLibrary.DocumentAssetAsync(documentAssetId);
    }

    public string DocumentAsset(IDocumentAssetRenderable asset)
    {
        return _documentAssetRouteLibrary.DocumentAsset(asset);
    }

    public Task<string> DocumentAssetDownloadAsync(int? documentAssetId)
    {
        return _documentAssetRouteLibrary.DocumentAssetDownloadAsync(documentAssetId);
    }

    public string DocumentAssetDownload(IDocumentAssetRenderable asset)
    {
        return _documentAssetRouteLibrary.DocumentAssetDownload(asset);
    }

    #endregion

    public string ToAbsolute(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return url;

        return _siteUriResolver.MakeAbsolute(url);
    }
}
