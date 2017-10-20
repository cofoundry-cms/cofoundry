using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityDefinitionMicroSummaryByIdQueryHandler 
        : IAsyncQueryHandler<GetByStringQuery<CustomEntityDefinitionMicroSummary>, CustomEntityDefinitionMicroSummary>
        , IIgnorePermissionCheckHandler
    {
        #region constructor 

        private readonly IEnumerable<ICustomEntityDefinition> _customEntityRegistrations;
        private readonly ICustomEntityDefinitionMicroSummaryMapper _customEntityDefinitionMicroSummaryMapper;

        public GetCustomEntityDefinitionMicroSummaryByIdQueryHandler(
            IEnumerable<ICustomEntityDefinition> customEntityRegistrations,
            ICustomEntityDefinitionMicroSummaryMapper customEntityDefinitionMicroSummaryMapper
            )
        {
            _customEntityRegistrations = customEntityRegistrations;
            _customEntityDefinitionMicroSummaryMapper = customEntityDefinitionMicroSummaryMapper;
        }

        #endregion

        #region execution

        public Task<CustomEntityDefinitionMicroSummary> ExecuteAsync(GetByStringQuery<CustomEntityDefinitionMicroSummary> query, IExecutionContext executionContext)
        {
            var definition = _customEntityRegistrations.SingleOrDefault(d => d.CustomEntityDefinitionCode == query.Id);
            var result = _customEntityDefinitionMicroSummaryMapper.Map(definition);

            return Task.FromResult(result);
        }

        #endregion
    }
}
