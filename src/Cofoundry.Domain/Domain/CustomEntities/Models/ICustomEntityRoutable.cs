﻿namespace Cofoundry.Domain;

/// <summary>
/// An custom entity object with information about how to construct 
/// a url to a page representing that custom entity.
/// </summary>
public interface ICustomEntityRoutable
{
    /// <summary>
    /// If this custom entity has page routes asspciated with it
    /// they will be included here. Typically you'd only expect a
    /// single page on a site to be associated with a custom entitiy, 
    /// but it's technically possible to have many.
    /// </summary>
    IReadOnlyCollection<string> PageUrls { get; set; }
}
