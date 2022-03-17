namespace Cofoundry.Domain;

/// <summary>
/// A repository to make it easier to get instances of custom entity 
/// definitions registered in the DI container.
/// </summary>
public interface ICustomEntityDefinitionRepository
{
    /// <summary>
    /// Returns a custom entity definition by it's unique 
    /// <see cref="ICustomEntityDefinition.CustomEntityDefinitionCode"/>. If
    /// the definition does not exist then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="customEntityDefinitionCode">The unique 6 character code that identifies the custom entity definition.</param>
    /// <returns>Returns the matching <see cref="ICustomEntityDefinition"/> instance if one is found; otherwise <see langword="null"/>.</returns>
    ICustomEntityDefinition GetByCode(string customEntityDefinitionCode);

    /// <summary>
    /// Returns a custom entity definition by it's unique 
    /// <see cref="ICustomEntityDefinition.CustomEntityDefinitionCode"/>. If the definition 
    /// does not exist then an <see cref="EntityNotFoundException{IUserAreaDefinition}"/>
    /// is thrown.
    /// </summary>
    /// <param name="customEntityDefinitionCode">The unique 6 character code that identifies the custom entity definition.</param>
    /// <exception cref="EntityNotFoundException{IUserAreaDefinition}">Thrown if the definition could not be found.</exception>
    /// <returns>Returns the matching <see cref="ICustomEntityDefinition"/> instance.</returns>
    ICustomEntityDefinition GetRequiredByCode(string customEntityDefinitionCode);

    /// <summary>
    /// Returns all custom entity definitions instances registered in the DI container.
    /// </summary>
    IEnumerable<ICustomEntityDefinition> GetAll();

    /// <summary>
    /// Returns a custom entity definition instance by it's type. If the definition 
    /// does not exist then an <see cref="EntityNotFoundException{TDefinition}"/> is
    /// thrown.
    /// </summary>
    /// <typeparam name="TDefinition">The type of definition to find.</typeparam>
    /// <exception cref="EntityNotFoundException{TDefinition}">Thrown if the definition could not be found.</exception>
    /// <returns>Returns the matching <see cref="ICustomEntityDefinition"/> instance.</returns>
    ICustomEntityDefinition GetRequired<TDefinition>() where TDefinition : ICustomEntityDefinition;
}
