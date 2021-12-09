using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving all yser area definitions.
    /// </summary>
    public interface IContentRepositoryUserAreaGetAllQueryBuilder
    {
        /// <summary>
        /// Returns a collection of all user areas registered with the system
        /// as a lightweight <see cref="UserAreaMicroSummary"/> projection, which 
        /// contains only basic identification properties such as code and name.
        /// </summary>
        IDomainRepositoryQueryContext<ICollection<UserAreaMicroSummary>> AsMicroSummaries();
    }
}