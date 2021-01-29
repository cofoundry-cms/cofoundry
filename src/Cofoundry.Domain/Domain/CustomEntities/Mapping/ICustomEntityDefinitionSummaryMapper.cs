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
    public interface ICustomEntityDefinitionSummaryMapper
    {
        /// <summary>
        /// Maps a code base custom entity definition into a CustomEntityDefinitionSummary object.
        /// </summary>
        /// <param name="codeDefinition">Code based definition to map.</param>
        CustomEntityDefinitionSummary Map(ICustomEntityDefinition codeDefinition);
    }
}
