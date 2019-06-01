using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Query to get a custom entity definition by it's unique 6 character code.
    /// The returned projection contains much of the same data as the source 
    /// defintion class, but the main difference is that instead of using generics 
    /// to identify the data model type, there is instead a DataModelType property.
    /// </summary>
    public class GetCustomEntityDefinitionSummaryByCodeQuery : IQuery<CustomEntityDefinitionSummary>
    {
        /// <summary>
        /// Query to get a custom entity definition by it's unique 6 character code.
        /// The returned projection contains much of the same data as the source 
        /// defintion class, but the main difference is that instead of using generics 
        /// to identify the data model type, there is instead a DataModelType property.
        /// </summary>
        public GetCustomEntityDefinitionSummaryByCodeQuery()
        {
        }

        /// <summary>
        /// Query to get a custom entity definition by it's unique 6 character code.
        /// The returned projection contains much of the same data as the source 
        /// defintion class, but the main difference is that instead of using generics 
        /// to identify the data model type, there is instead a DataModelType property.
        /// </summary>
        /// <param name="customEntityDefinitionCode">Unique 6 letter code representing the entity.</param>
        public GetCustomEntityDefinitionSummaryByCodeQuery(string customEntityDefinitionCode)
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        /// <summary>
        /// Unique 6 letter code representing the entity.
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }
    }
}
