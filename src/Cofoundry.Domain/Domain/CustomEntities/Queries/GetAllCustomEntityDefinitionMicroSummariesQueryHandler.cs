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
        : IAsyncQueryHandler<GetAllQuery<CustomEntityDefinitionMicroSummary>, IEnumerable<CustomEntityDefinitionMicroSummary>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly ICustomEntityDefinitionRepository _customEntityDefinitionRepository;

        public GetAllCustomEntityDefinitionMicroSummariesQueryHandler(
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            _customEntityDefinitionRepository = customEntityDefinitionRepository;
        }

        #endregion

        #region execution

        public Task<IEnumerable<CustomEntityDefinitionMicroSummary>> ExecuteAsync(GetAllQuery<CustomEntityDefinitionMicroSummary> query, IExecutionContext executionContext)
        {
            var result = Mapper.Map<IEnumerable<CustomEntityDefinitionMicroSummary>>(_customEntityDefinitionRepository.GetAll());

            return Task.FromResult(result);
        }

        #endregion
    }
}
