using Cofoundry.Web.Framework.Mvc.ViewHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="ICofoundryHelper{TModel}"/>.
/// </summary>
public class CofoundryPageHelper<TModel> : ICofoundryHelper<TModel>, IViewContextAware
{
    public CofoundryPageHelper(
        IContentRouteLibrary contentRouteLibrary,
        IStaticFileViewHelper staticFileViewHelper,
        ISettingsViewHelper settings,
        ICurrentUserViewHelper currentUser,
        IJavascriptViewHelper js,
        IHtmlSanitizerHelper sanitizer,
        ICofoundryHtmlHelper html
        )
    {
        Routing = contentRouteLibrary;
        StaticFiles = staticFileViewHelper;
        Settings = settings;
        CurrentUser = currentUser;
        Js = js;
        Sanitizer = sanitizer;
        Html = html;
    }

    private TModel? _model;
    /// <summary>
    /// The view model associated with the page this helper is contained in.
    /// </summary>
    public TModel Model
    {
        get
        {
            if (_model == null)
            {
                throw ViewHelperNotContextualizedException.Create<CofoundryPageHelper<TModel>>(nameof(Model));
            }
            return _model;
        }
    }

    /// <inheritdoc/>
    public IContentRouteLibrary Routing { get; }

    /// <inheritdoc/>
    public IStaticFileViewHelper StaticFiles { get; }

    /// <inheritdoc/>
    public ISettingsViewHelper Settings { get; }

    /// <inheritdoc/>
    public ICurrentUserViewHelper CurrentUser { get; }

    /// <inheritdoc/>
    public IJavascriptViewHelper Js { get; }

    /// <inheritdoc/>
    public IHtmlSanitizerHelper Sanitizer { get; }

    /// <inheritdoc/>
    public ICofoundryHtmlHelper Html { get; }

    public virtual void Contextualize(ViewContext viewContext)
    {
        if (viewContext.ViewData.Model is TModel model)
        {
            _model = model;
        }
        else if (viewContext.ViewData.Model != null)
        {
            throw new Exception($"The view model type '{viewContext.ViewData.Model?.GetType().Name}' does not match the generic type parameter '{typeof(TModel).Name}'");
        }
    }
}
