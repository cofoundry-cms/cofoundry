using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns a complete tree of page directory nodes, starting
    /// with the root directory as a single node.
    /// </summary>
    public class GetPageDirectoryTreeQueryHandler 
        : IAsyncQueryHandler<GetPageDirectoryTreeQuery, PageDirectoryNode>
        , IPermissionRestrictedQueryHandler<GetPageDirectoryTreeQuery, PageDirectoryNode>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetPageDirectoryTreeQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution
        
        public async Task<PageDirectoryNode> ExecuteAsync(GetPageDirectoryTreeQuery query, IExecutionContext executionContext)
        {
            var allPageDirectories = await Query().ToListAsync();
            return Map(allPageDirectories);
        }

        #endregion

        #region helpers

        private IQueryable<PageDirectoryNode> Query()
        {
            var allPageDirectories = _dbContext
                   .PageDirectories
                   .AsNoTracking()
                   .Include(w => w.Creator)
                   .Where(w => w.IsActive)
                   .ProjectTo<PageDirectoryNode>();

            return allPageDirectories;
        }

        private PageDirectoryNode Map(List<PageDirectoryNode> allPageDirectories)
        {
            if (allPageDirectories == null) return null;

            // Build the urls
            var root = allPageDirectories.SingleOrDefault(r => !r.ParentPageDirectoryId.HasValue);
            EntityNotFoundException.ThrowIfNull(root, "ROOT");
            SetChildRoutes(root, allPageDirectories);
            root.FullUrlPath = "/";

            return root;
        }

        private void SetChildRoutes(PageDirectoryNode parent, List<PageDirectoryNode> allRoutes)
        {
            var childNodes = new List<PageDirectoryNode>();
            foreach (var routingInfo in allRoutes.Where(r => r.ParentPageDirectoryId == parent.PageDirectoryId))
            {
                routingInfo.FullUrlPath = string.Join("/", parent.FullUrlPath, routingInfo.UrlPath);
                routingInfo.Depth = parent.Depth + 1;
                routingInfo.ParentPageDirectory = parent;
                childNodes.Add(routingInfo);

                SetChildRoutes(routingInfo, allRoutes);
            }

            parent.ChildPageDirectories = childNodes;
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageDirectoryTreeQuery command)
        {
            yield return new PageDirectoryReadPermission();
        }

        #endregion
    }
}
