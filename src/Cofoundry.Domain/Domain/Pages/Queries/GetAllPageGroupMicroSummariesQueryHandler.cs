using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    public class GetAllPageGroupMicroSummariesQueryHandler
        : IQueryHandler<GetAllPageGroupMicroSummariesQuery, ICollection<PageGroupMicroSummary>>
        , IPermissionRestrictedQueryHandler<GetAllPageGroupMicroSummariesQuery, ICollection<PageGroupMicroSummary>>
    {
        private readonly CofoundryDbContext _dbContext;

        public GetAllPageGroupMicroSummariesQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task<ICollection<PageGroupMicroSummary>> ExecuteAsync(GetAllPageGroupMicroSummariesQuery query, IExecutionContext executionContext)
        {
            var results = await _dbContext
                .PageGroups
                .AsNoTracking()
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

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllPageGroupMicroSummariesQuery query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
