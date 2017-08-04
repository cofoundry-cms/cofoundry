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
        : IAsyncQueryHandler<GetAllQuery<CustomEntityDefinitionSummary>, IEnumerable<CustomEntityDefinitionSummary>>
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

        public Task<IEnumerable<CustomEntityDefinitionSummary>> ExecuteAsync(GetAllQuery<CustomEntityDefinitionSummary> query, IExecutionContext executionContext)
        {
            var result = Mapper.Map<IEnumerable<CustomEntityDefinitionSummary>>(_customEntityDefinitionRepository.GetAll());

            return Task.FromResult(result);
        }

        #endregion
    }
}
