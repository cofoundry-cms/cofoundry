using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to CustomEntityDefinitionMicroSummary objects.
    /// </summary>
    public class CustomEntityDefinitionMicroSummaryMapper : ICustomEntityDefinitionMicroSummaryMapper
    {
        /// <summary>
        /// Maps a code base custom entity definition into a CustomEntityDefinitionMicroSummary object.
        /// </summary>
        /// <param name="codeDefinition">Code based definition to map.</param>
        public CustomEntityDefinitionMicroSummary Map(ICustomEntityDefinition codeDefinition)
        {
            if (codeDefinition == null) throw new ArgumentNullException(nameof(codeDefinition));

            var result = new CustomEntityDefinitionMicroSummary()
            {
                CustomEntityDefinitionCode = codeDefinition.CustomEntityDefinitionCode,
                Description = codeDefinition.Description,
                ForceUrlSlugUniqueness = codeDefinition.ForceUrlSlugUniqueness,
                Name = codeDefinition.Name,
                NamePlural = codeDefinition.NamePlural
            };

            return result;
        }

        /// <summary>
        /// Maps a CustomEntityDefinitionSummary into a CustomEntityDefinitionMicroSummary object.
        /// </summary>
        /// <param name="summary">Instance to map.</param>
        public CustomEntityDefinitionMicroSummary Map(CustomEntityDefinitionSummary summary)
        {
            if (summary == null) throw new ArgumentNullException(nameof(summary));

            var result = new CustomEntityDefinitionMicroSummary()
            {
                CustomEntityDefinitionCode = summary.CustomEntityDefinitionCode,
                Description = summary.Description,
                ForceUrlSlugUniqueness = summary.ForceUrlSlugUniqueness,
                Name = summary.Name,
                NamePlural = summary.NamePlural
            };

            return result;
        }
    }
}
