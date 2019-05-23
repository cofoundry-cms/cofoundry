using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class GetNestedDataModelSchemaByNameRangeQueryHandler
        : IAsyncQueryHandler<GetNestedDataModelSchemaByNameRangeQuery, IDictionary<string, NestedDataModelSchema>>
        , IIgnorePermissionCheckHandler
    {
        #region constructor

        private readonly INestedDataModelSchemaMapper _nestedDataModelSchemaMapper;
        private readonly INestedDataModelTypeRepository _nestedDataModelRepository;

        public GetNestedDataModelSchemaByNameRangeQueryHandler(
            INestedDataModelSchemaMapper nestedDataModelSchemaMapper,
            INestedDataModelTypeRepository nestedDataModelRepository
            )
        {
            _nestedDataModelSchemaMapper = nestedDataModelSchemaMapper;
            _nestedDataModelRepository = nestedDataModelRepository;
        }

        #endregion

        public Task<IDictionary<string, NestedDataModelSchema>> ExecuteAsync(GetNestedDataModelSchemaByNameRangeQuery query, IExecutionContext executionContext)
        {
            IDictionary<string, NestedDataModelSchema> result = null;

            if (EnumerableHelper.IsNullOrEmpty(query.Names)) return Task.FromResult(result);

            result = new Dictionary<string, NestedDataModelSchema>(StringComparer.OrdinalIgnoreCase);

            foreach (var name in query.Names.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var dataModelType = _nestedDataModelRepository.GetByName(name);
                NestedDataModelSchema schema = null;
                if (dataModelType != null)
                {
                    schema = _nestedDataModelSchemaMapper.Map(dataModelType);
                }
                result.Add(name, schema);
            }

            return Task.FromResult(result);
        }
    }
}
