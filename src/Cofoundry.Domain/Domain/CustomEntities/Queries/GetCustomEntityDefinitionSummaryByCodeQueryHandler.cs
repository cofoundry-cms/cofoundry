using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityDefinitionSummaryByCodeQueryHandler 
        : IQueryHandler<GetByStringQuery<CustomEntityDefinitionSummary>, CustomEntityDefinitionSummary>
        , IAsyncQueryHandler<GetByStringQuery<CustomEntityDefinitionSummary>, CustomEntityDefinitionSummary>
        , IIgnorePermissionCheckHandler
    {
        private readonly ICustomEntityCodeDefinitionRepository _customEntityDefinitionRepository;

        public GetCustomEntityDefinitionSummaryByCodeQueryHandler(
            ICustomEntityCodeDefinitionRepository customEntityDefinitionRepository
            )
        {
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        public CustomEntityDefinitionSummary Execute(GetByStringQuery<CustomEntityDefinitionSummary> query, IExecutionContext executionContext)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.Id.ToUpperInvariant());

            return Mapper.Map<CustomEntityDefinitionSummary>(definition);
        }

        public async Task<CustomEntityDefinitionSummary> ExecuteAsync(GetByStringQuery<CustomEntityDefinitionSummary> query, IExecutionContext executionContext)
        {
            var result = Execute(query, executionContext);

            return await Task.FromResult(result);
        }
    }
}
