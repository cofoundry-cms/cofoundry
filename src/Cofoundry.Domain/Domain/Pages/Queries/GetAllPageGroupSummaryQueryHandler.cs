using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    public class GetAllPageGroupSummaryQueryHandler 
        : IAsyncQueryHandler<GetAllQuery<PageGroupSummary>, IEnumerable<PageGroupSummary>>
        , IPermissionRestrictedQueryHandler<GetAllQuery<PageGroupSummary>, IEnumerable<PageGroupSummary>>
    {
        private readonly CofoundryDbContext _dbContext;

        public GetAllPageGroupSummaryQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<PageGroupSummary>> ExecuteAsync(GetAllQuery<PageGroupSummary> query, IExecutionContext executionContext)
        {
            var results = await _dbContext
                          .PageGroups
                          .AsNoTracking()
                          .Where(g => !g.IsDeleted)
                          .OrderBy(m => m.GroupName)
                          .ProjectTo<PageGroupSummary>()
                          .ToListAsync();

            return results;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllQuery<PageGroupSummary> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
