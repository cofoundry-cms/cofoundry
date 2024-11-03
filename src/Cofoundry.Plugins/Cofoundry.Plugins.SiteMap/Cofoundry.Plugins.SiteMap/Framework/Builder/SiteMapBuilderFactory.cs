using Cofoundry.Core.Validation;
using Cofoundry.Core.Web;

namespace Cofoundry.Plugins.SiteMap;

/// <summary>
/// Default factory for creating concrete <see cref="ISiteMapBuilder"/>
/// objects. Override this implementation to return a custom <see cref="SiteMapBuilder"/>
/// instance if you want more control over how the site map is constructed.
/// </summary>
public class SiteMapBuilderFactory : ISiteMapBuilderFactory
{
    private readonly ISiteUrlResolver _uriResolver;
    private readonly IModelValidationService _modelValidationService;

    public SiteMapBuilderFactory(
        ISiteUrlResolver uriResolver,
        IModelValidationService modelValidationService
        )
    {
        _uriResolver = uriResolver;
        _modelValidationService = modelValidationService;
    }

    /// <inheritdoc/>
    public ISiteMapBuilder Create()
    {
        return new SiteMapBuilder(_uriResolver, _modelValidationService);
    }
}
