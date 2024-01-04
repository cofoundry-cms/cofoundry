using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageDirectoryMicroSummaryMapper"/>.
/// </summary>
public class PageDirectoryMicroSummaryMapper : IPageDirectoryMicroSummaryMapper
{
    /// <inheritdoc/>
    [return: NotNullIfNotNull(nameof(pageDirectory))]
    public PageDirectoryMicroSummary? Map(PageDirectory? pageDirectory)
    {
        if (pageDirectory == null)
        {
            return null;
        }

        MissingIncludeException.ThrowIfNull(pageDirectory, c => c.PageDirectoryPath);

        var result = new PageDirectoryMicroSummary()
        {
            Depth = pageDirectory.PageDirectoryPath.Depth,
            FullUrlPath = "/" + pageDirectory.PageDirectoryPath.FullUrlPath,
            Name = pageDirectory.Name,
            PageDirectoryId = pageDirectory.PageDirectoryId,
            ParentPageDirectoryId = pageDirectory.ParentPageDirectoryId
        };

        return result;
    }
}
