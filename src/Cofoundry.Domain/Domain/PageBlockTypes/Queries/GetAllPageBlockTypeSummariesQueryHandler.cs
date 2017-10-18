using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetAllPageBlockTypeSummariesQueryHandler
        : IAsyncQueryHandler<GetAllQuery<PageBlockTypeSummary>, IEnumerable<PageBlockTypeSummary>>
        , IPermissionRestrictedQueryHandler<GetAllQuery<PageBlockTypeSummary>, IEnumerable<PageBlockTypeSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageBlockTypeCache _pageBlockTypeCache;
        private readonly IPageBlockTypeSummaryMapper _pageBlockTypeSummaryMapper;

        public GetAllPageBlockTypeSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IPageBlockTypeCache pageBlockTypeCache,
            IPageBlockTypeSummaryMapper pageBlockTypeSummaryMapper
            )
        {
            _dbContext = dbContext;
            _pageBlockTypeCache = pageBlockTypeCache;
            _pageBlockTypeSummaryMapper = pageBlockTypeSummaryMapper;
        }

        #endregion

        #region execution
        
        public async Task<IEnumerable<PageBlockTypeSummary>> ExecuteAsync(GetAllQuery<PageBlockTypeSummary> query, IExecutionContext executionContext)
        {
            return await _pageBlockTypeCache.GetOrAddAsync(async () =>
            {
                var dbResults = await Query().ToListAsync();
                var results = dbResults
                    .Select(_pageBlockTypeSummaryMapper.Map)
                    .ToArray();

                return results;
            });
        }

        private IQueryable<PageBlockType> Query()
        {
            var results = _dbContext
                .PageBlockTypes
                .AsNoTracking()
                .Include(t => t.PageBlockTemplates)
                .OrderBy(m => m.Name);

            return results;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllQuery<PageBlockTypeSummary> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
