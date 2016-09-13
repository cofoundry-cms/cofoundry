using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetAllCustomEntityDefinitionMicroSummariesQueryHandler 
        : IQueryHandler<GetAllQuery<CustomEntityDefinitionMicroSummary>, IEnumerable<CustomEntityDefinitionMicroSummary>>
        , IAsyncQueryHandler<GetAllQuery<CustomEntityDefinitionMicroSummary>, IEnumerable<CustomEntityDefinitionMicroSummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly ICustomEntityCodeDefinitionRepository _customEntityDefinitionRepository;

        public GetAllCustomEntityDefinitionMicroSummariesQueryHandler(
            ICustomEntityCodeDefinitionRepository customEntityDefinitionRepository
            )
        {
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        #region execution

        public IEnumerable<CustomEntityDefinitionMicroSummary> Execute(GetAllQuery<CustomEntityDefinitionMicroSummary> query, IExecutionContext executionContext)
        {
            return Mapper.Map<CustomEntityDefinitionMicroSummary[]>(_customEntityDefinitionRepository.GetAll());
        }

        public async Task<IEnumerable<CustomEntityDefinitionMicroSummary>> ExecuteAsync(GetAllQuery<CustomEntityDefinitionMicroSummary> query, IExecutionContext executionContext)
        {
            var result = Execute(query, executionContext);

            return await Task.FromResult(result);
        }

        #endregion
    }
}
