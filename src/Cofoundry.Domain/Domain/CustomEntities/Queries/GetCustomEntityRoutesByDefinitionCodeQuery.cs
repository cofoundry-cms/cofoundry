using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets CustomEntityRoute data for all custom entities of a 
    /// specific type. These route objects are small and cached which
    /// makes them good for quick lookups.
    /// </summary>
    public class GetCustomEntityRoutesByDefinitionCodeQuery : IQuery<ICollection<CustomEntityRoute>>
    {
        /// <summary>
        /// Gets CustomEntityRoute data for all custom entities of a 
        /// specific type. These route objects are small and cached which
        /// makes them good for quick lookups.
        /// </summary>
        public GetCustomEntityRoutesByDefinitionCodeQuery() { }

        /// <summary>
        /// Gets CustomEntityRoute data for all custom entities of a 
        /// specific type. These route objects are small and cached which
        /// makes them good for quick lookups.
        /// </summary>
        /// <param name="customEntityDefinitionCode">
        /// The code identifier for the custom entity type
        /// to query for.
        /// </param>
        public GetCustomEntityRoutesByDefinitionCodeQuery(string customEntityDefinitionCode)
        {
            CustomEntityDefinitionCode = customEntityDefinitionCode;
        }

        /// <summary>
        /// The code identifier for the custom entity type
        /// to query for.
        /// </summary>
        [Required]
        [MaxLength(6)]
        public string CustomEntityDefinitionCode { get; set; }
    }
}
