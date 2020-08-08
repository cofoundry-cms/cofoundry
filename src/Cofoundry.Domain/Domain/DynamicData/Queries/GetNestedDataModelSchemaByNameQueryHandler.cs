using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    public class GetNestedDataModelSchemaByNameQueryHandler
        : IQueryHandler<GetNestedDataModelSchemaByNameQuery, NestedDataModelSchema>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly IDynamicDataModelSchemaMapper _dynamicDataModelTypeMapper;
        private readonly INestedDataModelTypeRepository _nestedDataModelRepository;

        public GetNestedDataModelSchemaByNameQueryHandler(
            IDynamicDataModelSchemaMapper dynamicDataModelTypeMapper,
            INestedDataModelTypeRepository nestedDataModelRepository
            )
        {
            _dynamicDataModelTypeMapper = dynamicDataModelTypeMapper;
            _nestedDataModelRepository = nestedDataModelRepository;
        }

        #endregion

        #region execution

        public Task<NestedDataModelSchema> ExecuteAsync(GetNestedDataModelSchemaByNameQuery query, IExecutionContext executionContext)
        {
            NestedDataModelSchema result = null;

            if (string.IsNullOrWhiteSpace(query.Name)) return Task.FromResult(result);
            
            var dataModelType = _nestedDataModelRepository.GetByName(query.Name);

            if (dataModelType == null) return Task.FromResult(result);

            result = new NestedDataModelSchema();

            _dynamicDataModelTypeMapper.Map(result, dataModelType);

            return Task.FromResult(result);
        }

        #endregion
    }
}
