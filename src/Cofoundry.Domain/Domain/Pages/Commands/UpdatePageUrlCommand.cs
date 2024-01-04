namespace Cofoundry.Domain;

/// <summary>
/// Updates the url of a page. This is a separate command from
/// the UpdatePageCommand as it's a specific action that can
/// have specific side effects such as breaking page links outside
/// of the CMS.
/// </summary>
public class UpdatePageUrlCommand : ICommand, ILoggableCommand
{
    /// <summary>
    /// Id of the page to update.
    /// </summary>
    [PositiveInteger]
    [Required]
    public int PageId { get; set; }

    /// <summary>
    /// The directory to put the page in.
    /// </summary>
    [Required(ErrorMessage = "Please choose a directory")]
    [PositiveInteger]
    public int PageDirectoryId { get; set; }

    /// <summary>
    /// Optional id of the locale if used in a localized site.
    /// </summary>
    [PositiveInteger]
    public int? LocaleId { get; set; }

    /// <summary>
    /// The path of the page within the directory. This must be
    /// unique within the directory the page is parented to.
    /// E.g. 'about-the-team'. This can be <see langword="null"/>
    /// for <see cref="PageType.CustomEntityDetails"/>.
    /// </summary>
    [StringLength(200)]
    [Slug]
    public string? UrlPath { get; set; }

    /// <summary>
    /// If this is a <see cref="PageType.CustomEntityDetails"/> page, this will 
    /// need to be set to a value that matches the RouteFormat of an existing
    /// <see cref="ICustomEntityRoutingRule"/> e.g. "{Id}/{UrlSlug}".
    /// </summary>
    [StringLength(200)]
    public string? CustomEntityRoutingRule { get; set; }
}
