using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityDefinitionSummaryByCodeQueryHandler 
        : IAsyncQueryHandler<GetByStringQuery<CustomEntityDefinitionSummary>, CustomEntityDefinitionSummary>
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

        public async Task<CustomEntityDefinitionSummary> ExecuteAsync(GetByStringQuery<CustomEntityDefinitionSummary> query, IExecutionContext executionContext)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.Id.ToUpperInvariant());
            var result = _customEntityDefinitionSummaryMapper.Map(definition);

            return await Task.FromResult(result);
        }
    }
}
