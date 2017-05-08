using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;

namespace Cofoundry.Domain
{
    public class GetPageModuleTypeSummaryByIdHandler 
        : IAsyncQueryHandler<GetByIdQuery<PageModuleTypeSummary>, PageModuleTypeSummary>
        , IPermissionRestrictedQueryHandler<GetByIdQuery<PageModuleTypeSummary>, PageModuleTypeSummary>
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;

        public GetPageModuleTypeSummaryByIdHandler(
            IQueryExecutor queryExecutor
            )
        {
            _queryExecutor = queryExecutor;
        }

        #endregion

        public async Task<PageModuleTypeSummary> ExecuteAsync(GetByIdQuery<PageModuleTypeSummary> query, IExecutionContext executionContext)
        {
            var allModuleTypes = await _queryExecutor.GetAllAsync<PageModuleTypeSummary>();
            return allModuleTypes.SingleOrDefault(t => t.PageModuleTypeId == query.Id);
        }

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetByIdQuery<PageModuleTypeSummary> query)
        {
            yield return new PageReadPermission();
        }

        #endregion
    }
}
