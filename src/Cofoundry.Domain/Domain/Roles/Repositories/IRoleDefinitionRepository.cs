namespace Cofoundry.Domain;

/// <summary>
/// A repository to make it easier to get instances of role 
/// definitions registered in the DI container.
/// </summary>
public interface IRoleDefinitionRepository
{
    /// <summary>
    /// Returns a role definition by it's unique <see cref="IRoleDefinition.RoleCode"/>. 
    /// If the definition does not exist then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="userAreaCode">The unique 3 character code that identifies the user area the role belongs to.</param>
    /// <param name="roleCode">The unique 3 character code that identifies the role definition.</param>
    /// <returns>Returns the matching <see cref="IRoleDefinition"/> instance if one is found; otherwise <see langword="null"/>.</returns>
    IRoleDefinition GetByCode(string userAreaCode, string roleCode);

    /// <summary>
    /// Returns a role definition by it's unique <see cref="IRoleDefinition.RoleCode"/>. 
    /// If the definition does not exist then an <see cref="EntityNotFoundException{IRoleDefinition}"/>
    /// is thrown.
    /// </summary>
    /// <param name="userAreaCode">The unique 3 character code that identifies the user area the role belongs to.</param>
    /// <param name="roleCode">The unique 3 character code that identifies the role definition.</param>
    /// <exception cref="EntityNotFoundException{IRoleDefinition}">Thrown if the role definition could not be found.</exception>
    /// <returns>Returns the matching <see cref="IRoleDefinition"/> instance.</returns>
    IRoleDefinition GetRequiredByCode(string userAreaCode, string roleCode);

    /// <summary>
    /// Returns a role definition instance by it's type. If the definition 
    /// does not exist then an <see cref="EntityNotFoundException{TDefinition}"/> 
    /// is thrown.
    /// </summary>
    /// <typeparam name="TDefinition">The type of definition to find.</typeparam>
    /// <exception cref="EntityNotFoundException{TDefinition}">Thrown if the definition could not be found.</exception>
    /// <returns>Returns the matching <see cref="IRoleDefinition"/> instance.</returns>
    IRoleDefinition GetRequired<TDefinition>() where TDefinition : IRoleDefinition;

    /// <summary>
    /// Returns all role definitions instances registered in the DI container.
    /// </summary>
    IEnumerable<IRoleDefinition> GetAll();
}
