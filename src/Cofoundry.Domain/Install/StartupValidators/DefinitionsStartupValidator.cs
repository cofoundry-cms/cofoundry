using Cofoundry.Core.AutoUpdate.Internal;

namespace Cofoundry.Domain.Install;

/// <summary>
/// Validates against duplicate definitions and other invalid
/// properties.
/// </summary>
public class DefinitionsStartupValidator : IStartupValidator
{
    public DefinitionsStartupValidator(
        ICustomEntityDefinitionRepository customEntityDefinitionRepository,
        IAuthorizedTaskTypeDefinitionRepository authorizedTaskTypeDefinitionRepository,
        IRoleDefinitionRepository roleDefinitionRepository,
        IUserAreaDefinitionRepository userAreaDefinitionRepository
        )
    {
    }

    public void Validate()
    {
        // Definition repositories are singleton and will validate in the constructor
    }
}
