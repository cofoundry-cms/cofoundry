using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data.Internal
{
    /// <summary>
    /// Data access abstraction over stored procedures for user entities and associated records.
    /// </summary>
    public interface IUserStoredProcedures
    {
        /// <summary>
        /// Adds an email domain name if it doesn't exist, returning the Id of
        /// the existing or newly created record.
        /// </summary>
        /// <param name="name">
        /// The name of the domain, which should be in a valid lowercased
        /// format e.g. "example.com" or "müller.example.com".
        /// Maps to <see cref="EmailDomain.Name"/></param>
        /// <param name="uniqueName">
        /// A unique name for the domain that is used for lookups. This is hashed 
        /// and mapped to <see cref="EmailDomain.NameHash"/>.
        /// </param>
        /// <param name="dateNow">
        /// The current date and time, which is used as the create date if
        /// a new entity is created.
        /// </param>
        /// <returns>The Id of the existing or newly created record.</returns>
        Task<int> AddEmailDomainIfNotExistsAsync(
            string name, 
            string uniqueName,
            DateTime dateNow
            );
    }
}
