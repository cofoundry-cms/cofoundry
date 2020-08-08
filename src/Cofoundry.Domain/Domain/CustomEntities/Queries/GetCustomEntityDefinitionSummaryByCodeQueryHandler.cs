using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Query to get a custom entity definition by it's unique 6 character code.
    /// The returned projection contains much of the same data as the source 
    /// defintion class, but the main difference is that instead of using generics 
    /// to identify the data model type, there is instead a DataModelType property.
    /// </summary>
    public class GetCustomEntityDefinitionSummaryByCodeQueryHandler 
        : IQueryHandler<GetCustomEntityDefinitionSummaryByCodeQuery, CustomEntityDefinitionSummary>
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
