using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cofoundry.Web;

/// <summary>
/// Factory that enables ICustomEntityTemplateRegionTagBuilder implementation to be swapped out.
/// </summary>
/// <remarks>
/// The factory is required because the HtmlHelper cannot be injected
/// </remarks>
public interface ICustomEntityTemplateRegionTagBuilderFactory
{
    ICustomEntityTemplateRegionTagBuilder<TModel> Create<TModel>(
        ViewContext viewContext,
        ICustomEntityPageViewModel<TModel> customEntityViewModel,
        string regionName
        )
        where TModel : ICustomEntityPageDisplayModel
        ;
}
