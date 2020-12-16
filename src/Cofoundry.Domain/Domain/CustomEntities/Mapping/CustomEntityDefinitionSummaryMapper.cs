using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to CustomEntityDefinitionSummary objects.
    /// </summary>
    public class CustomEntityDefinitionSummaryMapper : ICustomEntityDefinitionSummaryMapper
    {
        /// <summary>
        /// Maps a code base custom entity definition into a CustomEntityDefinitionSummary object.
        /// </summary>
        /// <param name="codeDefinition">Code based definition to map.</param>
        public CustomEntityDefinitionSummary Map(ICustomEntityDefinition codeDefinition)
        {
            if (codeDefinition == null) throw new ArgumentNullException(nameof(codeDefinition));

            var result = new CustomEntityDefinitionSummary()
            {
                CustomEntityDefinitionCode = codeDefinition.CustomEntityDefinitionCode,
                Description = codeDefinition.Description,
                ForceUrlSlugUniqueness = codeDefinition.ForceUrlSlugUniqueness,
                Name = codeDefinition.Name,
                NamePlural = codeDefinition.NamePlural,
                AutoGenerateUrlSlug = codeDefinition.AutoGenerateUrlSlug,
                AutoPublish = codeDefinition.AutoPublish,
                HasLocale = codeDefinition.HasLocale,
                Ordering = CustomEntityOrdering.None
            };

            result.DataModelType = codeDefinition.GetDataModelType();
            result.Terms = codeDefinition.GetTerms();

            if (codeDefinition is IOrderableCustomEntityDefinition orderableDefinition)
            {
                result.Ordering = orderableDefinition.Ordering;
            }

            return result;
        }
    }
}
