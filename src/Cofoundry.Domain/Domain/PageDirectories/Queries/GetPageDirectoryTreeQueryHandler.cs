using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.QueryModels;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns a complete tree of page directory nodes, starting
    /// with the root directory as a single node.
    /// </summary>
    public class GetPageDirectoryTreeQueryHandler 
        : IQueryHandler<GetPageDirectoryTreeQuery, PageDirectoryNode>
        , IPermissionRestrictedQueryHandler<GetPageDirectoryTreeQuery, PageDirectoryNode>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageDirectoryTreeMapper _pageDirectoryTreeMapper;

        public GetPageDirectoryTreeQueryHandler(
            CofoundryDbContext dbContext,
            IPageDirectoryTreeMapper pageDirectoryTreeMapper
            )
        {
            _dbContext = dbContext;
            _pageDirectoryTreeMapper = pageDirectoryTreeMapper;
        }

        #endregion

        #region execution
        
        public async Task<PageDirectoryNode> ExecuteAsync(GetPageDirectoryTreeQuery query, IExecutionContext executionContext)
        {
            var dbResults = await Query().ToListAsync();
            var result = _pageDirectoryTreeMapper.Map(dbResults);

            return result;
        }

        private IQueryable<PageDirectoryTreeNodeQueryModel> Query()
        {
            var allPageDirectories = _dbContext
                   .PageDirectories
                   .AsNoTracking()
                   .Include(w => w.Creator)
                   .Select(d => new PageDirectoryTreeNodeQueryModel()
                    {
                       Creator = d.Creator,
                       PageDirectory = d,
                       NumPages = d.Pages.Count()
                   });

            return allPageDirectories;
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
