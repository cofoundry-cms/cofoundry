using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    public class CustomEntityDataModelMapper : ICustomEntityDataModelMapper
    {
        private readonly IEnumerable<ICustomEntityDefinition> _customEntityDefinitions;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;

        public CustomEntityDataModelMapper(
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            IEnumerable<ICustomEntityDefinition> customEntityDefinitions
            )
        {
            _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
            _customEntityDefinitions = customEntityDefinitions;
        }

        public ICustomEntityDataModel Map(string customEntityDefinitionCode, string serializedData)
        {
            var definition = _customEntityDefinitions.SingleOrDefault(d => d.CustomEntityDefinitionCode == customEntityDefinitionCode);
            EntityNotFoundException.ThrowIfNull(definition, customEntityDefinitionCode);

            var dataModelType = definition.GetDataModelType();

            return (ICustomEntityDataModel)_dbUnstructuredDataSerializer.Deserialize(serializedData, dataModelType);
        }
    }
}
