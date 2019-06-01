using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Query to get a custom entity definition by it's unique 6 character code.
    /// The returned object is a lightweight projection of the data defined in a custom entity 
    /// definition class. This is typically used as part of another domain model or
    /// for querying lists of definitions in the admin panel.
    /// </summary>
    public class GetCustomEntityDefinitionMicroSummaryByCodeQuery : IQuery<CustomEntityDefinitionMicroSummary>
    {
        /// <summary>
        /// Query to get a custom entity definition by it's unique 6 character code.
        /// The returned object is a lightweight projection of the data defined in a custom entity 
        /// definition class. This is typically used as part of another domain model or
        /// for querying lists of definitions in the admin panel.
        /// </summary>
        public GetCustomEntityDefinitionMicroSummaryByCodeQuery()
        {
        }

        /// <summary>
        /// Query to get a custom entity definition by it's unique 6 character code.
        /// The returned object is a lightweight projection of the data defined in a custom entity 
        /// definition class. This is typically used as part of another domain model or
        /// for querying lists of definitions in the admin panel.
        /// </summary>
        /// <param name="customEntityDefinitionCode">Unique 6 letter code representing the entity.</param>
        public GetCustomEntityDefinitionMicroSummaryByCodeQuery(string customEntityDefinitionCode)
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        /// <summary>
        /// Unique 6 letter code representing the entity.
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }
    }
}
