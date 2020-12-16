using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    public class EntityDefinitionRepository : IEntityDefinitionRepository
    {
        #region constructor

        private readonly Dictionary<string, IEntityDefinition> _entityDefinitions;

        public EntityDefinitionRepository(
            IEnumerable<IEntityDefinition> customEntityDefinitions,
            ICustomEntityDefinitionRepository customEntityDefinitionRepository
            )
        {
            var dynamicDefinitions = customEntityDefinitionRepository
                .GetAll()
                .Select(c => new CustomEntityDynamicEntityDefinition(c));

            var allDefinitions = customEntityDefinitions
                .Where(k => !(k is CustomEntityDynamicEntityDefinition))
                .Union(dynamicDefinitions);

            DetectInvalidDefinitions(allDefinitions);

            _entityDefinitions = allDefinitions.ToDictionary(k => k.EntityDefinitionCode);
        }

        private void DetectInvalidDefinitions(IEnumerable<IEntityDefinition> definitions)
        {
            const string WHY_VALID_CODE_MESSAGE = "All entity definition codes must be 6 characters and contain only non-unicode caracters.";

            var nullCode = definitions
                .Where(d => string.IsNullOrWhiteSpace(d.EntityDefinitionCode))
                .FirstOrDefault();

            if (nullCode != null)
            {
                var message = nullCode.GetType().Name + " does not have a definition code specified.";
                throw new InvalidEntityDefinitionException(message, nullCode, definitions);
            }

            var nullName = definitions
                .Where(d => string.IsNullOrWhiteSpace(d.Name))
                .FirstOrDefault();

            if (nullName != null)
            {
                var message = nullName.GetType().Name + " does not have a name specified.";
                throw new InvalidEntityDefinitionException(message, nullName, definitions);
            }

            var dulpicateCode = definitions
                .GroupBy(e => e.EntityDefinitionCode)
                .Where(g => g.Count() > 1)
                .FirstOrDefault();

            if (dulpicateCode != null)
            {
                var message = "Duplicate IEntityDefinition.EntityDefinitionCode: " + dulpicateCode.Key;
                throw new InvalidEntityDefinitionException(message, dulpicateCode.First(), definitions);
            }

            var dulpicateName = definitions
                .GroupBy(e => e.Name)
                .Where(g => g.Count() > 1)
                .FirstOrDefault();

            if (dulpicateName != null)
            {
                var message = "Duplicate IEntityDefinition.Name: " + dulpicateName.Key;
                throw new InvalidEntityDefinitionException(message, dulpicateName.First(), definitions);
            }

            var codeNot6Chars = definitions
                .Where(d => d.EntityDefinitionCode.Length != 6)
                .FirstOrDefault();

            if (codeNot6Chars != null)
            {
                var message = codeNot6Chars.GetType().Name + " has a definition code that is not 6 characters in length. " + WHY_VALID_CODE_MESSAGE;
                throw new InvalidEntityDefinitionException(message, codeNot6Chars, definitions);
            }

            var notValidCode = definitions
                .Where(d => !SqlCharValidator.IsValid(d.EntityDefinitionCode, 6))
                .FirstOrDefault();

            if (notValidCode != null)
            {
                var message = notValidCode.GetType().Name + " has an invalid definition code. " + WHY_VALID_CODE_MESSAGE;
                throw new InvalidEntityDefinitionException(message, notValidCode, definitions);
            }
        }

        #endregion

        public IEntityDefinition GetByCode(string code)
        {
            return _entityDefinitions.GetOrDefault(code);
        }

        public IEnumerable<IEntityDefinition> GetAll()
        {
            return _entityDefinitions.Values;
        }
    }
}
