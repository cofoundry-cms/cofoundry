using Cofoundry.Domain.QueryModels;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageDirectoryRouteMapper"/>.
/// </summary>
public class PageDirectoryTreeMapper : IPageDirectoryTreeMapper
{
    private readonly IAuditDataMapper _auditDataMapper;

    public PageDirectoryTreeMapper(
        IAuditDataMapper auditDataMapper
        )
    {
        _auditDataMapper = auditDataMapper;
    }


    /// <inheritdoc/>
    public PageDirectoryNode Map(IReadOnlyCollection<PageDirectoryTreeNodeQueryModel> dbPageDirectories)
    {
        var allPageDirectories = dbPageDirectories
            .Select(MapInitial)
            .ToArray();

        // Build the urls
        var root = allPageDirectories.SingleOrDefault(r => !r.ParentPageDirectoryId.HasValue);
        EntityNotFoundException.ThrowIfNull(root, "ROOT");
        SetChildRoutes(root, allPageDirectories);
        root.FullUrlPath = "/";

        return root;
    }

    private PageDirectoryNode MapInitial(PageDirectoryTreeNodeQueryModel queryModel)
    {
        var dbDirectory = queryModel.PageDirectory;
        var result = new PageDirectoryNode()
        {
            Name = dbDirectory.Name,
            NumPages = queryModel.NumPages,
            PageDirectoryId = dbDirectory.PageDirectoryId,
            ParentPageDirectoryId = dbDirectory.ParentPageDirectoryId,
            UrlPath = dbDirectory.UrlPath
        };

        result.AuditData = _auditDataMapper.MapCreateAuditData(dbDirectory);

        return result;
    }

    private static void SetChildRoutes(PageDirectoryNode parent, IReadOnlyCollection<PageDirectoryNode> allDirectories)
    {
        var childNodes = new List<PageDirectoryNode>();
        foreach (var directory in allDirectories.Where(r => r.ParentPageDirectoryId == parent.PageDirectoryId))
        {
            directory.FullUrlPath = string.Join("/", parent.FullUrlPath, directory.UrlPath);
            directory.Depth = parent.Depth + 1;
            directory.ParentPageDirectory = parent;
            childNodes.Add(directory);

            SetChildRoutes(directory, allDirectories);
        }

        parent.ChildPageDirectories = childNodes;
    }
}
