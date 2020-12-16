using Cofoundry.Core;
using Cofoundry.Domain.QueryModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// A mapper for mapping a tree structure of PageDirectoryNode objects.
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

        
        /// <summary>
        /// Maps a collection of projected EF PageDirectoryTreeNodeQueryModel records from the db 
        /// into a tree of PageDirectoryNode instances with a single root node
        /// object.
        /// </summary>
        /// <param name="dbPageDirectories">PageDirectoryTreeNodeQueryModel records from the database.</param>
        public PageDirectoryNode Map(IReadOnlyCollection<PageDirectoryTreeNodeQueryModel> dbPageDirectories)
        {
            var allPageDirectories = dbPageDirectories
                .Select(MapInitial)
                .ToList();

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

        private void SetChildRoutes(PageDirectoryNode parent, List<PageDirectoryNode> allDirectories)
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
}
