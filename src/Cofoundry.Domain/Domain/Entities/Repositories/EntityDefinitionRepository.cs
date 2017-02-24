using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class EntityDefinitionRepository : IEntityDefinitionRepository
    {
        #region constructor

        private readonly Dictionary<string, IEntityDefinition> _entityDefinitions;

        public EntityDefinitionRepository(
            IEntityDefinition[] customEntityDefinitions,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            var dynamicDefinitions = customEntityDefinitionRepository
                .GetAll()
                .Select(c => new CustomEntityDynamicEntityDefinition(c));

            _entityDefinitions = customEntityDefinitions
                .Where(k => !(k is CustomEntityDynamicEntityDefinition))
                .Union(dynamicDefinitions)
                .ToDictionary(k => k.EntityDefinitionCode);
        }

        #endregion

        public IEntityDefinition GetByCode(string code)
        {
            return _entityDefinitions.GetOrDefault(code);
        }
    }
}
