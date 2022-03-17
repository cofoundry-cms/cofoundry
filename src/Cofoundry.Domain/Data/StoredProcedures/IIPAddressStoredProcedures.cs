namespace Cofoundry.Domain.Data.Internal;

/// <summary>
/// Data access abstraction over stored procedures for IP address logging.
/// </summary>
public interface IIPAddressStoredProcedures
{
    /// <summary>
    /// Adds an IP Address if it doesn't exist, returning the id of
    /// the existing or newly created record.
    /// </summary>
    /// <param name="address">
    /// The textual representation of the IP Address, or a hash if IP hashing
    /// is enabled. Must be 45 characters or less.
    /// <param name="dateNow">
    /// The current date and time, which is used as the create date if
    /// a new entity is created.
    /// </param>
    /// <returns>The Id of the existing or newly created record.</returns>
    Task<int> AddIfNotExistsAsync(
        string address,
        DateTime dateNow
        );
}
