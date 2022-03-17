namespace Cofoundry.Web;

/// <summary>
/// Main helper for Cofoundry functionality on PageTemplate 
/// views. Typically accessed via @Cofoundry, this keeps 
/// all cofoundry functionality under one helper to avoid 
/// polluting the global namespace.
/// </summary>
public interface ICofoundryTemplateHelper<TModel>
    : ICofoundryHelper<TModel>
    where TModel : IEditablePageViewModel
{
    /// <summary>
    /// Contains helper functionality relating to the page template
    /// such as region definitions.
    /// </summary>
    IPageTemplateHelper<TModel> Template { get; }
}
