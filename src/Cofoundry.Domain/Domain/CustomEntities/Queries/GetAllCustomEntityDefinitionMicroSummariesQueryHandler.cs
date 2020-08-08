using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Query to get a collection of all custom entity definitions registered
    /// with the system. The returned object is a lightweight projection of the
    /// data defined in a custom entity definition class which is typically used 
    /// as part of another domain model.
    /// </summary>
    public class GetAllCustomEntityDefinitionMicroSummariesQueryHandler 
        : IQueryHandler<GetAllCustomEntityDefinitionMicroSummariesQuery, ICollection<CustomEntityDefinitionMicroSummary>>
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

        public Task<ICollection<CustomEntityDefinitionMicroSummary>> ExecuteAsync(GetAllCustomEntityDefinitionMicroSummariesQuery query, IExecutionContext executionContext)
        {
            var result = _customEntityDefinitionRepository
                .GetAll()
                .Select(_customEntityDefinitionMicroSummaryMapper.Map)
                .ToList();

            return Task.FromResult<ICollection<CustomEntityDefinitionMicroSummary>>(result);
        }
    }
}
