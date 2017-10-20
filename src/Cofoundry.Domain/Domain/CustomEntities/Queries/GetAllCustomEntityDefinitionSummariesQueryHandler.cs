using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetAllCustomEntityDefinitionSummariesQueryHandler 
        : IAsyncQueryHandler<GetAllQuery<CustomEntityDefinitionSummary>, IEnumerable<CustomEntityDefinitionSummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor 
        
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;
        private readonly ICustomEntityDefinitionSummaryMapper _customEntityDefinitionSummaryMapper;

        public GetAllCustomEntityDefinitionSummariesQueryHandler(
            ICustomEntityDefinitionRepository customEntityDefinitionRepository,
            ICustomEntityDefinitionSummaryMapper customEntityDefinitionSummaryMapper
            )
        {
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
            _customEntityDefinitionSummaryMapper = customEntityDefinitionSummaryMapper;
        }

        #endregion

        #region execution

        public Task<IEnumerable<CustomEntityDefinitionSummary>> ExecuteAsync(GetAllQuery<CustomEntityDefinitionSummary> query, IExecutionContext executionContext)
        {
           var results = _customEntityDefinitionRepository
                .GetAll()
                .Select(_customEntityDefinitionSummaryMapper.Map)
                .ToList()
                .AsEnumerable();

            return Task.FromResult(results);
        }

        #endregion
    }
}
