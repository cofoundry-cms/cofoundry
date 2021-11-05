using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class PageDirectoryMicroSummaryMapper : IPageDirectoryMicroSummaryMapper
    {
        public PageDirectoryMicroSummary Map(PageDirectory pageDirectory)
        {
            if (pageDirectory == null) return null;
            MissingIncludeException.ThrowIfNull(pageDirectory, c => c.PageDirectoryPath);

            var result = new PageDirectoryMicroSummary()
            {
                Depth = pageDirectory.PageDirectoryPath.Depth,
                FullUrlPath = pageDirectory.PageDirectoryPath.FullUrlPath,
                Name = pageDirectory.Name,
                PageDirectoryId = pageDirectory.PageDirectoryId,
                ParentPageDirectoryId = pageDirectory.ParentPageDirectoryId
            };

            return result;
        }
    }
}
