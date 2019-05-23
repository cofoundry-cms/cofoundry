using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets meta information about an INestedDataModel type using the 
    /// type name. The "DataModel" suffix is optional in the type name
    /// and the lookup is case-insesnitive.
    /// </summary>
    public class GetNestedDataModelSchemaByNameQuery : IQuery<NestedDataModelSchema>
    {
        public GetNestedDataModelSchemaByNameQuery()
        {
        }

        /// <summary>
        /// Initialized the query with parameters.
        /// </summary>
        /// <param name="name">
        /// The data model type name with or without the "DataModel" suffix,
        /// the lookup is case-insesnitive.
        /// </param>
        public GetNestedDataModelSchemaByNameQuery(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The data model type name with or without the "DataModel" suffix,
        /// the lookup is case-insesnitive.
        /// </summary>
        public string Name { get; set; }
    }
}
