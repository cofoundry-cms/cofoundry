using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityDefinitionMicroSummaryByIdQueryHandler 
        : IQueryHandler<GetByStringQuery<CustomEntityDefinitionMicroSummary>, CustomEntityDefinitionMicroSummary>
        , IAsyncQueryHandler<GetByStringQuery<CustomEntityDefinitionMicroSummary>, CustomEntityDefinitionMicroSummary>
        , IIgnorePermissionCheckHandler
    {
        #region constructor 

        private readonly ICustomEntityDefinition[] _customEntityModuleRegistrations;

        public GetCustomEntityDefinitionMicroSummaryByIdQueryHandler(
            ICustomEntityDefinition[] customEntityModuleRegistrations
            )
        {
            _customEntityModuleRegistrations = customEntityModuleRegistrations;
        }

        #endregion

        #region execution

        public CustomEntityDefinitionMicroSummary Execute(GetByStringQuery<CustomEntityDefinitionMicroSummary> query, IExecutionContext executionContext)
        {
            var definition = _customEntityModuleRegistrations.SingleOrDefault(d => d.CustomEntityDefinitionCode == query.Id);
            return Mapper.Map<CustomEntityDefinitionMicroSummary>(definition);
        }

        public async Task<CustomEntityDefinitionMicroSummary> ExecuteAsync(GetByStringQuery<CustomEntityDefinitionMicroSummary> query, IExecutionContext executionContext)
        {
            var result = Execute(query, executionContext);

            return await Task.FromResult(result);
        }

        #endregion
    }
}
