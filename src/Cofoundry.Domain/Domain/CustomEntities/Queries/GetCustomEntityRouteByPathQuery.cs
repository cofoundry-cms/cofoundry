﻿namespace Cofoundry.Domain;

/// <summary>
/// Looks up a route for a custom entity page using either the 
/// CustomEntityId or UrlSlug property. These route objects are 
/// cached in order to make routing lookups speedy.
/// </summary>
public class GetCustomEntityRouteByPathQuery : IQuery<CustomEntityRoute?>
{
    /// <summary>
    /// Unique 6 character code representing the type of custom 
    /// entity find.
    /// </summary>
    [Required]
    [MaxLength(6)]
    public string CustomEntityDefinitionCode { get; set; } = string.Empty;

    /// <summary>
    /// Database id of the custom entity to find. Either the
    /// id or url slug should be specified.
    /// </summary>
    public int? CustomEntityId { get; set; }

    /// <summary>
    /// Optional id of the locale if used in a localized site.
    /// </summary>
    public int? LocaleId { get; set; }

    /// <summary>
    /// The slug identifier of the custom entity to find. Either the
    /// id or url slug should be specified.
    /// </summary>
    public string? UrlSlug { get; set; }
}
