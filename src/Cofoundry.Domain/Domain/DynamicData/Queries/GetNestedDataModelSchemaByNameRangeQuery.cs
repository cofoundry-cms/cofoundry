using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets meta information about INestedDataModel types using the 
    /// type name. The "DataModel" suffix is optional in the type name
    /// and the lookup is case-insesnitive. The lookup name is used 
    /// unmodified as the key in the returned dictionary.
    /// </summary>
    public class GetNestedDataModelSchemaByNameRangeQuery : IQuery<IDictionary<string, NestedDataModelSchema>>
    {
        public GetNestedDataModelSchemaByNameRangeQuery()
        {
        }

        /// <summary>
        /// Initialized the query with parameters.
        /// </summary>
        /// <param name="names">
        /// The data model type names with or without the "DataModel" suffix.
        /// the lookup is case-insesnitive.
        /// </param>
        public GetNestedDataModelSchemaByNameRangeQuery(IEnumerable<string> names)
            : this(names?.ToList())
        {
        }

        /// <summary>
        /// Initialized the query with parameters.
        /// </summary>
        /// <param name="names">
        /// The data model type names with or without the "DataModel" suffix.
        /// the lookup is case-insesnitive.
        /// </param>
        public GetNestedDataModelSchemaByNameRangeQuery(
            IReadOnlyCollection<string> names
            )
        {
            if (names == null) throw new ArgumentNullException(nameof(names));

            Names = names;
        }

        /// <summary>
        /// The data model type names with or without the "DataModel" suffix.
        /// the lookup is case-insesnitive.
        /// </summary>
        public IReadOnlyCollection<string> Names { get; set; }
    }
}
