using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetAllCustomEntityDefinitionMicroSummariesQueryHandler 
        : IAsyncQueryHandler<GetAllQuery<CustomEntityDefinitionMicroSummary>, IEnumerable<CustomEntityDefinitionMicroSummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;
        private readonly ICustomEntityDefinitionMicroSummaryMapper _customEntityDefinitionMicroSummaryMapper;

        public GetAllCustomEntityDefinitionMicroSummariesQueryHandler(
            ICustomEntityDefinitionRepository customEntityDefinitionRepository,
            ICustomEntityDefinitionMicroSummaryMapper customEntityDefinitionMicroSummaryMapper
            )
        {
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
            _customEntityDefinitionMicroSummaryMapper = customEntityDefinitionMicroSummaryMapper;
        }

        #endregion

        #region execution

        public Task<IEnumerable<CustomEntityDefinitionMicroSummary>> ExecuteAsync(GetAllQuery<CustomEntityDefinitionMicroSummary> query, IExecutionContext executionContext)
        {
            var result = _customEntityDefinitionRepository
                .GetAll()
                .Select(_customEntityDefinitionMicroSummaryMapper.Map)
                .ToList()
                .AsEnumerable();

            return Task.FromResult(result);
        }

        #endregion
    }
}
