namespace Cofoundry.Domain;

/// <summary>
/// A repository to make it easier to get instances of authorized task
/// type definitions registered in the DI container.
/// </summary>
public interface IAuthorizedTaskTypeDefinitionRepository
{
    /// <summary>
    /// Returns an authorized task type definition by it's unique <see cref="IAuthorizedTaskTypeDefinition.AuthorizedTaskTypeCode"/>. 
    /// If the definition does not exist then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="authorizedTaskTypeCode">The unique 6 character code that identifies the authorized task type.</param>
    /// <returns>Returns the matching <see cref="IAuthorizedTaskTypeDefinition"/> instance, or <see langword="null"/> if it does not exist.</returns>
    IAuthorizedTaskTypeDefinition GetByCode(string authorizedTaskTypeCode);

    /// <summary>
    /// Returns an authorized task type definition by it's unique <see cref="IAuthorizedTaskTypeDefinition.AuthorizedTaskTypeCode"/>. 
    /// If the definition does not exist then an <see cref="EntityNotFoundException{IAuthorizedTaskTypeDefinition}"/>
    /// is thrown.
    /// </summary>
    /// <param name="authorizedTaskTypeCode">The unique 6 character code that identifies the authorized task type.</param>
    /// <exception cref="EntityNotFoundException{IAuthorizedTaskTypeDefinition}">Thrown if the authorized task type definition could not be found.</exception>
    /// <returns>Returns the matching <see cref="IAuthorizedTaskTypeDefinition"/> instance.</returns>
    IAuthorizedTaskTypeDefinition GetRequiredByCode(string authorizedTaskTypeCode);

    /// <summary>
    /// Returns all authorized task type definitions instances registered in the DI container.
    /// </summary>
    IEnumerable<IAuthorizedTaskTypeDefinition> GetAll();
}
