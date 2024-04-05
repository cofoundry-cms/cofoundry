using Cofoundry.Web.Framework.Mvc.ViewHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Web;

/// <summary>
/// Default implementation of <see cref="IPageTemplateHelper{TModel}"/>.
/// </summary>
public class PageTemplateHelper<TModel>
    : IPageTemplateHelper<TModel>, IViewContextAware
    where TModel : IEditablePageViewModel
{
    private ViewContext? _viewContext;
    /// <summary>
    /// The view model associated with the page this helper is contained in
    /// </summary>
    public ViewContext ViewContext
    {
        get
        {
            if (_viewContext == null)
            {
                throw ViewHelperNotContextualizedException.Create<CofoundryPageHelper<TModel>>(nameof(ViewContext));
            }
            return _viewContext;
        }
    }

    private TModel? _model;
    /// <summary>
    /// The view model associated with the page this helper is contained in
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

    public void Contextualize(ViewContext viewContext)
    {
        _viewContext = viewContext;

        if (viewContext.ViewData.Model is TModel model)
        {
            if (!(model is IEditablePageViewModel))
            {
                throw new ArgumentException("Page templates must use a model that inherits from " + typeof(IEditablePageViewModel).Name);
            }
            _model = model;
        }
        else if (viewContext.ViewData.Model is null)
        {
            throw new Exception($"Null model supplied when contextualizing '{nameof(CofoundryPageHelper<TModel>)}'.");
        }
        else
        {
            var modelType = viewContext.ViewData.Model.GetType();
            throw new Exception($"Incorrect model type supplied when contextualizing '{nameof(CofoundryPageHelper<TModel>)}' with model type '{modelType.FullName}'.");
        }
    }

    /// <inheritdoc/>
    public IPageTemplateRegionTagBuilder Region(string regionName)
    {
        var factory = ViewContext.HttpContext.RequestServices.GetRequiredService<IPageTemplateRegionTagBuilderFactory>();
        var output = factory.Create(ViewContext, Model, regionName);

        return output;
    }

    /// <inheritdoc/>
    public IHtmlContent UseDescription(string description)
    {
        ArgumentNullException.ThrowIfNull(description);

        // nothing is rendered here, this is just used as a convention for adding template meta data
        return HtmlString.Empty;
    }
}
