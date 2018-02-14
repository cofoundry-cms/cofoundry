using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetAllCustomEntityDefinitionSummariesQueryHandler 
        : IAsyncQueryHandler<GetAllCustomEntityDefinitionSummariesQuery, ICollection<CustomEntityDefinitionSummary>>
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

        public Task<ICollection<CustomEntityDefinitionSummary>> ExecuteAsync(GetAllCustomEntityDefinitionSummariesQuery query, IExecutionContext executionContext)
        {
           var results = _customEntityDefinitionRepository
                .GetAll()
                .Select(_customEntityDefinitionSummaryMapper.Map)
                .ToList();

            return Task.FromResult<ICollection<CustomEntityDefinitionSummary>>(results);
        }

        #endregion
    }
}
