using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using System.Data.Entity;

namespace Cofoundry.Domain
{
    public class GetAllPageModuleTypeSummariesQueryHandler
        : IQueryHandler<GetAllQuery<PageModuleTypeSummary>, IEnumerable<PageModuleTypeSummary>>
        , IAsyncQueryHandler<GetAllQuery<PageModuleTypeSummary>, IEnumerable<PageModuleTypeSummary>>
        , IPermissionRestrictedQueryHandler<GetAllQuery<PageModuleTypeSummary>, IEnumerable<PageModuleTypeSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IPageModuleTypeCache _moduleCache;

        public GetAllPageModuleTypeSummariesQueryHandler(
            CofoundryDbContext dbContext,
            IPageModuleTypeCache moduleCache
            )
        {
            _dbContext = dbContext;
            _moduleCache = moduleCache;
        }

        #endregion

        #region execution

        public IEnumerable<PageModuleTypeSummary> Execute(GetAllQuery<PageModuleTypeSummary> query, IExecutionContext executionContext)
        {
            return _moduleCache.GetOrAdd(() => 
                {
                    var results = Query().ToArray();

                    return results;
                });
        }

        public async Task<IEnumerable<PageModuleTypeSummary>> ExecuteAsync(GetAllQuery<PageModuleTypeSummary> query, IExecutionContext executionContext)
        {
            return await _moduleCache.GetOrAddAsync(() =>
            {
                var results = Query().ToArrayAsync();

                return results;
            });
        }

        #endregion

        #region helpers

        private IQueryable<PageModuleTypeSummary> Query()
        {
            var results = _dbContext
                .PageModuleTypes
                .AsNoTracking()
                .OrderBy(m => m.Name)
                .ProjectTo<PageModuleTypeSummary>();

            return results;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllQuery<PageModuleTypeSummary> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
