using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityDefinitionSummaryByCodeQueryHandler 
        : IAsyncQueryHandler<GetCustomEntityDefinitionSummaryByCodeQuery, CustomEntityDefinitionSummary>
        , IIgnorePermissionCheckHandler
    {
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;
        private readonly ICustomEntityDefinitionSummaryMapper _customEntityDefinitionSummaryMapper;

        public GetCustomEntityDefinitionSummaryByCodeQueryHandler(
            ICustomEntityDefinitionRepository customEntityDefinitionRepository,
            ICustomEntityDefinitionSummaryMapper customEntityDefinitionSummaryMapper
            )
        {
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
            _customEntityDefinitionSummaryMapper = customEntityDefinitionSummaryMapper;
        }

        public Task<CustomEntityDefinitionSummary> ExecuteAsync(GetCustomEntityDefinitionSummaryByCodeQuery query, IExecutionContext executionContext)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.CustomEntityDefinitionCode.ToUpperInvariant());
            var result = _customEntityDefinitionSummaryMapper.Map(definition);

            return Task.FromResult(result);
        }
    }
}
