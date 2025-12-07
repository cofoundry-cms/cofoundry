namespace Dev.Sandbox;

/// <summary>
/// An ICustomEntityDetailsDisplayViewModel implementation is required if
/// you want to use a page template to dynamically render a details view
/// of a custom entity. This provides us with a strongly typed model to use
/// in the template.
/// </summary>
public class BlogPostDisplayModel : ICustomEntityPageDisplayModel<BlogPostDataModel>
{
    public required string PageTitle { get; set; }

    public required string? MetaDescription { get; set; }

    public required string Title { get; set; }

    public required DateTime Date { get; set; }

    public required string FullPath { get; set; }

    public required AuthorDetails? Author { get; set; }

    public required IReadOnlyCollection<CategorySummary> Categories { get; set; }
}
