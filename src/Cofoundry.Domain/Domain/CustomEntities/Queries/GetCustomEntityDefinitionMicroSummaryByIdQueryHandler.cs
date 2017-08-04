using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetCustomEntityDefinitionMicroSummaryByIdQueryHandler 
        : IAsyncQueryHandler<GetByStringQuery<CustomEntityDefinitionMicroSummary>, CustomEntityDefinitionMicroSummary>
        , IIgnorePermissionCheckHandler
    {
        #region constructor 

        private readonly IEnumerable<ICustomEntityDefinition> _customEntityRegistrations;

        public GetCustomEntityDefinitionMicroSummaryByIdQueryHandler(
            IEnumerable<ICustomEntityDefinition> customEntityRegistrations
            )
        {
            _customEntityRegistrations = customEntityRegistrations;
        }

        public IEnumerable<ICustomEntityDefinition> CustomEntityRegistrations => _customEntityRegistrations;

        #endregion

        #region execution

        public Task<CustomEntityDefinitionMicroSummary> ExecuteAsync(GetByStringQuery<CustomEntityDefinitionMicroSummary> query, IExecutionContext executionContext)
        {
            var definition = _customEntityRegistrations.SingleOrDefault(d => d.CustomEntityDefinitionCode == query.Id);
            var result = Mapper.Map<CustomEntityDefinitionMicroSummary>(definition);

            return Task.FromResult(result);
        }

        #endregion
    }
}
