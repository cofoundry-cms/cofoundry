using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class CustomEntityCodeDefinitionRepository : ICustomEntityCodeDefinitionRepository
    {
        #region constructor

        private readonly Dictionary<string, ICustomEntityDefinition> _customEntityDefinitions;

        public CustomEntityCodeDefinitionRepository(
            ICustomEntityDefinition[] customEntityDefinitions
            )
        {
            DetectDuplicates(customEntityDefinitions);
            _customEntityDefinitions = customEntityDefinitions.ToDictionary(k => k.CustomEntityDefinitionCode);
        }

        private void DetectDuplicates(ICustomEntityDefinition[] definitions)
        {
            var dulpicateCodes = definitions
                .GroupBy(e => e.CustomEntityDefinitionCode)
                .Where(g => g.Count() > 1);

            if (dulpicateCodes.Any())
            {
                throw new ApplicationException("Duplicate ICustomEntityModuleDefinition.CustomEntityDefinitionCode: " + dulpicateCodes.First().Key);
            }

            var dulpicateNames = definitions
                .GroupBy(e => e.Name)
                .Where(g => g.Count() > 1);

            if (dulpicateNames.Any())
            {
                throw new ApplicationException("Duplicate ICustomEntityModuleDefinition.Name: " + dulpicateCodes.First().Key);
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
    }
}
