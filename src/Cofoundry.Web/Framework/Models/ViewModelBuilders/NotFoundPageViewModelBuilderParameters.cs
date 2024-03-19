﻿namespace Cofoundry.Web;

/// <summary>
/// Parameters for mapping an INotFoundPageViewModel using an
/// IPageViewModelBuilder implementation.
/// </summary>
/// <remarks>
/// It's possible for the mapper to be overriden and customized so 
/// this parameter class helps future proof the design in case we 
/// want to add more parameters in the future.
/// </remarks>
public class NotFoundPageViewModelBuilderParameters
{
    /// <summary>
    /// Base path i.e. virtual directory of the requested path.
    /// </summary>
    public string PathBase { get; set; } = string.Empty;

    /// <summary>
    /// The path of the page requested that could not be found.
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Full querystring of the requested path with leading ? character.
    /// </summary>
    public string? QueryString { get; set; }
}
