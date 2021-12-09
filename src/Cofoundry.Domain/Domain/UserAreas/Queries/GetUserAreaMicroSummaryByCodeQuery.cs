using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns a user area by it's unique <see cref="IUserAreaDefinition.UserAreaCode"/>,
    /// projected as a <see cref="UserAreaMicroSummary"/> model. If the definition does not exist 
    /// then <see langword="null"/> is returned.
    /// </summary>
    public class GetUserAreaMicroSummaryByCodeQuery : IQuery<UserAreaMicroSummary>
    {
        /// <summary>
        /// Initislized a new <see cref="GetUserAreaMicroSummaryByCodeQuery"/> instance.
        /// </summary>
        public GetUserAreaMicroSummaryByCodeQuery() { }

        /// <summary>
        /// Initializes the query with parameters.
        /// </summary>
        /// <param name="userAreaCode">
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area to query.
        /// </param>
        public GetUserAreaMicroSummaryByCodeQuery(string userAreaCode)
        {
            UserAreaCode = userAreaCode;
        }

        /// <summary>
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area to query.
        /// </summary>
        [Required]
        [StringLength(3)]
        public string UserAreaCode { get; set; }
    }
}
