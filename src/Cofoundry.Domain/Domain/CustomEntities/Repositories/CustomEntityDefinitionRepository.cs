using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    public class CustomEntityDefinitionRepository : ICustomEntityDefinitionRepository
    {
        #region constructor

        private readonly Dictionary<string, ICustomEntityDefinition> _customEntityDefinitions;

        public CustomEntityDefinitionRepository(
            IEnumerable<ICustomEntityDefinition> customEntityDefinitions
            )
        {
            DetectInvalidDefinitions(customEntityDefinitions);
            _customEntityDefinitions = customEntityDefinitions.ToDictionary(k => k.CustomEntityDefinitionCode);
        }

        private void DetectInvalidDefinitions(IEnumerable<ICustomEntityDefinition> definitions)
        {
            const string WHY_VALID_CODE_MESSAGE = "All custom entity definition codes must be 6 characters and contain only non-unicode caracters.";

            var nullCode = definitions
                .Where(d => string.IsNullOrWhiteSpace(d.CustomEntityDefinitionCode))
                .FirstOrDefault();

            if (nullCode != null)
            {
                var message = nullCode.GetType().Name + " does not have a definition code specified.";
                throw new InvalidCustomEntityDefinitionException(message, nullCode, definitions);
            }

            var nullName = definitions
                .Where(d => string.IsNullOrWhiteSpace(d.Name))
                .FirstOrDefault();

            if (nullName != null)
            {
                var message = nullName.GetType().Name + " does not have a name specified.";
                throw new InvalidCustomEntityDefinitionException(message, nullName, definitions);
            }

            var dulpicateCode = definitions
                .GroupBy(e => e.CustomEntityDefinitionCode)
                .Where(g => g.Count() > 1)
                .FirstOrDefault();

            if (dulpicateCode != null)
            {
                var message = "Duplicate ICustomEntityDefinition.CustomEntityDefinitionCode: " + dulpicateCode.Key;
                throw new InvalidCustomEntityDefinitionException(message, dulpicateCode.First(), definitions);
            }

            var dulpicateName = definitions
                .GroupBy(e => e.Name)
                .Where(g => g.Count() > 1)
                .FirstOrDefault();

            if (dulpicateName != null)
            {
                var message = "Duplicate ICustomEntityDefinition.Name: " + dulpicateName.Key;
                throw new InvalidCustomEntityDefinitionException(message, dulpicateName.First(), definitions);
            }

            var nameNot6Chars = definitions
                .Where(d => d.CustomEntityDefinitionCode.Length != 6)
                .FirstOrDefault();

            if (nameNot6Chars != null)
            {
                var message = nameNot6Chars.GetType().Name + " has a definition code that is not 6 characters in length. " + WHY_VALID_CODE_MESSAGE;
                throw new InvalidCustomEntityDefinitionException(message, nameNot6Chars, definitions);
            }

            var notValidCode = definitions
                .Where(d => !SqlCharValidator.IsValid(d.CustomEntityDefinitionCode, 6))
                .FirstOrDefault();

            if (notValidCode != null)
            {
                var message = notValidCode.GetType().Name + " has an invalid definition code. " + WHY_VALID_CODE_MESSAGE;
                throw new InvalidCustomEntityDefinitionException(message, notValidCode, definitions);
            }
        }

        #endregion

        public ICustomEntityDefinition GetByCode(string code)
        {
            return _customEntityDefinitions.GetOrDefault(code);
        }

        public IEnumerable<ICustomEntityDefinition> GetAll()
        {
            return _customEntityDefinitions.Select(p => p.Value);
        }

        public ICustomEntityDefinition Get<TDefinition>()
            where TDefinition : ICustomEntityDefinition
        {
            return _customEntityDefinitions
                .Select(p => p.Value)
                .FirstOrDefault(p => p is TDefinition);
        }
    }
}
