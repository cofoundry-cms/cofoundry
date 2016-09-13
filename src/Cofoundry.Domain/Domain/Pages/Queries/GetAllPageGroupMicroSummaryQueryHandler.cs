using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using AutoMapper.QueryableExtensions;

namespace Cofoundry.Domain
{
    public class GetAllPageGroupMicroSummaryQueryHandler
        : IQueryHandler<GetAllQuery<PageGroupMicroSummary>, IEnumerable<PageGroupMicroSummary>>
        , IPermissionRestrictedQueryHandler<GetAllQuery<PageGroupMicroSummary>, IEnumerable<PageGroupMicroSummary>>
    {
        private readonly CofoundryDbContext _dbContext;

        public GetAllPageGroupMicroSummaryQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public IEnumerable<PageGroupMicroSummary> Execute(GetAllQuery<PageGroupMicroSummary> query, IExecutionContext executionContext)
        {
            var results = _dbContext
                          .PageGroups
                          .AsNoTracking()
                          .Where(g => !g.IsDeleted)
                          .OrderBy(m => m.GroupName)
                          .ProjectTo<PageGroupMicroSummary>()
                          .ToList();

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
