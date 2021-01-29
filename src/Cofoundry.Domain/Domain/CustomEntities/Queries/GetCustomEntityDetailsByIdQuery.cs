using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Query for detailed information on a custom entity and it's latest version. This 
    /// query is primarily used in the admin area because it is not version-specific
    /// and the CustomEntityDetails projection includes audit data and other additional 
    /// information that should normally be hidden from a customer facing app.
    /// </summary>
    public class GetCustomEntityDetailsByIdQuery : IQuery<CustomEntityDetails>
    {
        /// <summary>
        /// Query for detailed information on a custom entity and it's latest version. This 
        /// query is primarily used in the admin area because it is not version-specific
        /// and the CustomEntityDetails projection includes audit data and other additional 
        /// information that should normally be hidden from a customer facing app.
        /// </summary>
        public GetCustomEntityDetailsByIdQuery() { }

        /// <summary>
        /// Query for detailed information on a custom entity and it's latest version. This 
        /// query is primarily used in the admin area because it is not version-specific
        /// and the CustomEntityDetails projection includes audit data and other additional 
        /// information that should normally be hidden from a customer facing app.
        /// </summary>
        /// <param name="customEntityId">Id of the custom entity to find.</param>
        public GetCustomEntityDetailsByIdQuery(int customEntityId)
        {
            CustomEntityId = customEntityId;
        }

        /// <summary>
        /// Id of the custom entity to find.
        /// </summary>
        public int CustomEntityId { get; set; }
    }
}
