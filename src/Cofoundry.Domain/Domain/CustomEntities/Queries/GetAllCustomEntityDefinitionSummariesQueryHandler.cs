using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetAllCustomEntityDefinitionSummariesQueryHandler 
        : IQueryHandler<GetAllQuery<CustomEntityDefinitionSummary>, IEnumerable<CustomEntityDefinitionSummary>>
        , IAsyncQueryHandler<GetAllQuery<CustomEntityDefinitionSummary>, IEnumerable<CustomEntityDefinitionSummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor 
        
        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public GetAllCustomEntityDefinitionSummariesQueryHandler(
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        #region execution

        public IEnumerable<CustomEntityDefinitionSummary> Execute(GetAllQuery<CustomEntityDefinitionSummary> query, IExecutionContext executionContext)
        {
            return Mapper.Map<CustomEntityDefinitionSummary[]>(_customEntityDefinitionRepository.GetAll());
        }

        public async Task<IEnumerable<CustomEntityDefinitionSummary>> ExecuteAsync(GetAllQuery<CustomEntityDefinitionSummary> query, IExecutionContext executionContext)
        {
            var result = Execute(query, executionContext);

            return await Task.FromResult(result);
        }

        #endregion
    }
}
