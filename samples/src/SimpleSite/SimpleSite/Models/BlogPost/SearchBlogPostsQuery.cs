namespace SimpleSite;

/// <summary>
/// Inheriting from SimplePageableQuery or IPageableQuery 
/// gives us a few extra features when working with pages 
/// data via the PagingExtensions set of extension methods.
/// 
/// See https://www.cofoundry.org/docs/framework/data-access/paged-queries.cs
/// </summary>
public class SearchBlogPostsQuery : SimplePageableQuery
{
    public int CategoryId { get; set; }
}
