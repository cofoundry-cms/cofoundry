using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for accessing the currently logged in user. If
    /// there are multiple users then this only applies to the
    /// UserArea set as the default schema.
    /// </summary>
    public interface IContentRepositoryCurrentUserQueryBuilder
    {
        /// <summary>
        /// The UserMicroSummary is a minimal projection of user data 
        /// that is quick to load. If the user is not logged in then null #
        /// is returned.
        /// </summary>
        IDomainRepositoryQueryContext<UserMicroSummary> AsMicroSummary();

        /// <summary>
        /// The UserSummary is a reduced representation of a user. Building on 
        /// the UserMicroSummary, this projection contains additional audit 
        /// and basic role data. If the user is not logged in then null is returned.
        /// </summary>
        IDomainRepositoryQueryContext<UserSummary> AsSummary();

        /// <summary>
        /// The UserDetails projection is a full representation of a user, containing 
        /// all properties including role and permission data. If the user is not logged 
        /// in then null is returned.
        /// </summary>
        IDomainRepositoryQueryContext<UserDetails> AsDetails();
    }
}
