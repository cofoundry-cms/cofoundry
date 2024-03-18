﻿namespace Cofoundry.Domain;

/// <summary>
/// Adds a new page with a draft version and optionally publishes it.
/// </summary>
public class AddPageCommand : ICommand, ILoggableCommand, IValidatableObject
{
    /// <summary>
    /// The path of the page within the directory. This must be
    /// unique within the directory the page is parented to.
    /// E.g. 'about-the-team'. This can be <see langword="null"/>
    /// for <see cref="PageType.CustomEntityDetails"/>.
    /// </summary>
    [Display(Name = "Url path", Description = "Lower case and containing only letter, numbers, underscores and hyphens. E.g. 'about-the-team'")]
    [StringLength(200)]
    [Slug]
    public string? UrlPath { get; set; }

    /// <summary>
    /// The id of the directory the page should be added to.
    /// </summary>
    [Display(Name = "Directory")]
    [Required(ErrorMessage = "Please choose a directory")]
    [PositiveInteger]
    public int PageDirectoryId { get; set; }

    /// <summary>
    /// Optional id of the locale if used in a localized site.
    /// </summary>
    [Display(Name = "Locale")]
    [PositiveInteger]
    public int? LocaleId { get; set; }

    /// <summary>
    /// Id of the template used to render the page.
    /// </summary>
    [Required(ErrorMessage = "Please choose a page template")]
    [Display(Name = "Template")]
    [PositiveInteger]
    public int PageTemplateId { get; set; }

    /// <summary>
    /// The descriptive human-readable title of the page that is often 
    /// used in the html page title meta tag. Does not have to be
    /// unique.
    /// </summary>
    [Display(Description = "A few words descriptive page title, e.g. 'About the team'. Google <a href=\"http://en.wikipedia.org/wiki/Search_engine_results_page\" target=\"_blank\">SERP</a> typically shows 50-60 characters")]
    [StringLength(300)]
    [Required]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Tags can be used to categorize an entity.
    /// </summary>
    [Display(Name = "Tags", Description = "Separate each tag with a space: dog animal canine. Or to join 2 words together in one tag, use double quotes: \"golden retriever\". Used internally for searching for things.")]
    public IReadOnlyCollection<string> Tags { get; set; } = Array.Empty<string>();

    /// <summary>
    /// The description of the content of the page. This is intended to
    /// be used in the description html meta tag.
    /// </summary>
    [Display(Name = "Meta description", Description = "Ideally 25-250 characters. The Google <a href=\"http://en.wikipedia.org/wiki/Search_engine_results_page\" target=\"_blank\">SERP</a> shows only the first 150 characters")]
    [StringLength(300)]
    public string? MetaDescription { get; set; }

    /// <summary>
    /// Indicates whether the page should show in the auto-generated site map
    /// that gets presented to search engine robots.
    /// </summary>
    [Display(Name = "Show in site map?")]
    public bool ShowInSiteMap { get; set; }

    /// <summary>
    /// Indicates whether to publish the page immediately
    /// after adding it.
    /// </summary>
    public bool Publish { get; set; }

    /// <summary>
    /// If the Publish property is set to true then this optional field
    /// can be used to set a time that the page should be published and 
    /// made public. If this is left null then the publish date is set to 
    /// the current date and the page is made immediately available.
    /// </summary>
    public DateTime? PublishDate { get; set; }

    /// <summary>
    /// Most pages are generic pages but they could have some sort of
    /// special function e.g. NotFound, CustomEntityDetails.
    /// </summary>
    [Required]
    public PageType PageType { get; set; }

    /// <summary>
    /// If creating a <see cref="PageType.CustomEntityDetails"/> page, this 
    /// will need to be set to a value that matches the RouteFormat of an existing
    /// <see cref="ICustomEntityRoutingRule"/> e.g. "{Id}/{UrlSlug}".
    /// </summary>
    [StringLength(200)]
    public string? CustomEntityRoutingRule { get; set; }

    /// <summary>
    /// A title that can be used to share on social media via open 
    /// graph meta tags.
    /// </summary>
    [Display(Name = "Open graph title", Description = "Optional. The title that shows up when sharing the page on social media")]
    [StringLength(300)]
    public string? OpenGraphTitle { get; set; }

    /// <summary>
    /// A description that can be used to share on social media via open 
    /// graph meta tags.
    /// </summary>
    [Display(Name = "Open graph description", Description = "Optional. The description that shows up when sharing the page on social media")]
    public string? OpenGraphDescription { get; set; }

    /// <summary>
    /// An image that can be used to share on social media via open 
    /// graph meta tags.
    /// </summary>
    [Display(Name = "Open graph image", Description = "Optional. An image to show up when sharing the page on social media.")]
    [Image]
    [PositiveInteger]
    public int? OpenGraphImageId { get; set; }

    /// <summary>
    /// The database id of the newly created page. This is set after the 
    /// command has been run.
    /// </summary>
    [OutputValue]
    public int OutputPageId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PageType == PageType.CustomEntityDetails)
        {
            if (string.IsNullOrWhiteSpace(CustomEntityRoutingRule))
            {
                yield return new ValidationResult("A routing rule is required for custom entity details page types.", new[] { nameof(CustomEntityRoutingRule) });
            }
            if (!string.IsNullOrEmpty(UrlPath))
            {
                yield return new ValidationResult("Custom entity details pages should not specify a Url Path, instead they should specify a Routing Rule.", new[] { nameof(UrlPath) });
            }
        }

        if (PageType != PageType.CustomEntityDetails && !string.IsNullOrEmpty(CustomEntityRoutingRule))
        {
            yield return new ValidationResult("Custom Entity routing rules should only be specified for custom entity details page types.", new[] { nameof(CustomEntityRoutingRule) });
        }

        if (!Publish && PublishDate.HasValue)
        {
            yield return new ValidationResult("A publish date can only be set when a page is published.", new[] { nameof(PublishDate) });
        }
    }
}
