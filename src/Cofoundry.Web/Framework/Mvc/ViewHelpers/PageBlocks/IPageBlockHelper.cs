﻿namespace Cofoundry.Web;

/// <summary>
/// This helper exposes page block specific functionality in a page 
/// block type view file.
/// </summary>
public interface IPageBlockHelper<TViewModel>
{
    /// <summary>
    /// Sets a custom display name for the page block type. Usually the name is taken 
    /// from the class name e.g. 'RawHtmlDataModel' becomes 'Raw Html', but this 
    /// method allows you to override it e.g. 'Html'
    /// </summary>
    /// <param name="name">The text to use as the display name (max 50 characters)</param>
    IPageBlockHelper<TViewModel> UseDisplayName(string name);

    /// <summary>
    /// Set a custom description for the page block type or page block type template. This is useful 
    /// to describe the block type's functionality to users and help them decide which block type or template
    /// to choose when multiple are available.
    /// </summary>
    /// <param name="description">A plain text description about this block type or block type template</param>
    IPageBlockHelper<TViewModel> UseDescription(string description);
}
