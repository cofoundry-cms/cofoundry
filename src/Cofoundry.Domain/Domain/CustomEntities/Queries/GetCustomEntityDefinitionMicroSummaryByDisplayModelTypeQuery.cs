using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Query to get a custom entity definition by display model type definition.
    /// The returned object is a lightweight projection of the data defined in a custom entity 
    /// definition class and is typically used as part of another domain model.
    /// </summary>
    public class GetCustomEntityDefinitionMicroSummaryByDisplayModelTypeQuery : IQuery<CustomEntityDefinitionMicroSummary> 
    {
        /// <summary>
        /// Type definition of the concrete ICustomEntityDisplayModel 
        /// implementation to find the associated custom entity 
        /// definition for. 
        /// </summary>
        public Type DisplayModelType { get; set; }
    }
}
