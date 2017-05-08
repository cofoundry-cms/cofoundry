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
        : IAsyncQueryHandler<GetByStringQuery<CustomEntityDefinitionSummary>, CustomEntityDefinitionSummary>
        , IIgnorePermissionCheckHandler
    {
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public GetCustomEntityDefinitionSummaryByCodeQueryHandler(
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        public async Task<CustomEntityDefinitionSummary> ExecuteAsync(GetByStringQuery<CustomEntityDefinitionSummary> query, IExecutionContext executionContext)
        {
            var definition = _customEntityDefinitionRepository.GetByCode(query.Id.ToUpperInvariant());
            var result =  Mapper.Map<CustomEntityDefinitionSummary>(definition);

            return await Task.FromResult(result);
        }
    }
}
