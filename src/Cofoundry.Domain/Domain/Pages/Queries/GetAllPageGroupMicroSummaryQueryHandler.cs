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
    public class GetAllPageGroupMicroSummaryQueryHandler
        : IAsyncQueryHandler<GetAllQuery<PageGroupMicroSummary>, IEnumerable<PageGroupMicroSummary>>
        , IPermissionRestrictedQueryHandler<GetAllQuery<PageGroupMicroSummary>, IEnumerable<PageGroupMicroSummary>>
    {
        private readonly CofoundryDbContext _dbContext;

        public GetAllPageGroupMicroSummaryQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<PageGroupMicroSummary>> ExecuteAsync(GetAllQuery<PageGroupMicroSummary> query, IExecutionContext executionContext)
        {
            var results = await _dbContext
                .PageGroups
                .AsNoTracking()
                .Where(g => !g.IsDeleted)
                .OrderBy(m => m.GroupName)
                .Select(g => new PageGroupMicroSummary()
                {
                    Name = g.GroupName,
                    PageGroupId = g.PageGroupId,
                    ParentGroupId = g.ParentGroupId
                })
                .ToListAsync();

            return results;
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllQuery<PageGroupMicroSummary> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
