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
    /// Returns a complete tree of web directory nodes, starting
    /// with the root webdirectory as a single node.
    /// </summary>
    public class GetWebDirectoryTreeQueryHandler 
        : IQueryHandler<GetWebDirectoryTreeQuery, WebDirectoryNode>
        , IAsyncQueryHandler<GetWebDirectoryTreeQuery, WebDirectoryNode>
        , IPermissionRestrictedQueryHandler<GetWebDirectoryTreeQuery, WebDirectoryNode>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetWebDirectoryTreeQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public WebDirectoryNode Execute(GetWebDirectoryTreeQuery query, IExecutionContext executionContext)
        {
            var allWebDirectories = Query().ToList();
            return Map(allWebDirectories);
        }

        public async Task<WebDirectoryNode> ExecuteAsync(GetWebDirectoryTreeQuery query, IExecutionContext executionContext)
        {
            var allWebDirectories = await Query().ToListAsync();
            return Map(allWebDirectories);
        }

        #endregion

        #region helpers

        private IQueryable<WebDirectoryNode> Query()
        {
            var allWebDirectories = _dbContext
                   .WebDirectories
                   .AsNoTracking()
                   .Include(w => w.Creator)
                   .Where(w => w.IsActive)
                   .ProjectTo<WebDirectoryNode>();

            return allWebDirectories;
        }

        private WebDirectoryNode Map(List<WebDirectoryNode> allWebDirectories)
        {
            if (allWebDirectories == null) return null;

            // Build the urls
            var root = allWebDirectories.SingleOrDefault(r => !r.ParentWebDirectoryId.HasValue);
            EntityNotFoundException.ThrowIfNull(root, "ROOT");
            SetChildRoutes(root, allWebDirectories);
            root.FullUrlPath = "/";

            return root;
        }

        private void SetChildRoutes(WebDirectoryNode parent, List<WebDirectoryNode> allRoutes)
        {
            var childNodes = new List<WebDirectoryNode>();
            foreach (var routingInfo in allRoutes.Where(r => r.ParentWebDirectoryId == parent.WebDirectoryId))
            {
                routingInfo.FullUrlPath = string.Join("/", parent.FullUrlPath, routingInfo.UrlPath);
                routingInfo.Depth = parent.Depth + 1;
                routingInfo.ParentWebDirectory = parent;
                childNodes.Add(routingInfo);

                SetChildRoutes(routingInfo, allRoutes);
            }

            parent.ChildWebDirectories = childNodes;
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetWebDirectoryTreeQuery command)
        {
            yield return new WebDirectoryReadPermission();
        }

        #endregion
    }
}
