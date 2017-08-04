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

        public GetAllPageBlockTypeSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IPageBlockTypeCache pageBlockTypeCache
            )
        {
            _dbContext = dbContext;
            _pageBlockTypeCache = pageBlockTypeCache;
        }

        #endregion

        #region execution
        
        public async Task<IEnumerable<PageBlockTypeSummary>> ExecuteAsync(GetAllQuery<PageBlockTypeSummary> query, IExecutionContext executionContext)
        {
            return await _pageBlockTypeCache.GetOrAddAsync(() =>
            {
                var results = Query().ToArrayAsync();

                return results;
            });
        }

        private IQueryable<PageBlockTypeSummary> Query()
        {
            var results = _dbContext
                .PageBlockTypes
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .ProjectTo<PageBlockTypeSummary>();

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
