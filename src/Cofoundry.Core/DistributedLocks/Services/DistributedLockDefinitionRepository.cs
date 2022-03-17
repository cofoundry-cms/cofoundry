namespace Cofoundry.Core.DistributedLocks.Internal;

/// <summary>
/// A thin repository that validates and exposes all
/// the IDistributedLockDefinition instaces registered 
/// with Cofoundry. This should be registered as Singleton
/// so the validation is only done once.
/// </summary>
public class DistributedLockDefinitionRepository : IDistributedLockDefinitionRepository
{
    private readonly Dictionary<Type, IDistributedLockDefinition> _definitionLookup;

    public DistributedLockDefinitionRepository(
        IEnumerable<IDistributedLockDefinition> distributedLockDefinitions
        )
    {
        DetectInvalidDefinitions(distributedLockDefinitions);

        _definitionLookup = distributedLockDefinitions.ToDictionary(k => k.GetType());
    }

    private void DetectInvalidDefinitions(IEnumerable<IDistributedLockDefinition> definitions)
    {
        var dulpicateIds = definitions
            .GroupBy(e => e.DistributedLockId, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .FirstOrDefault();

        if (dulpicateIds != null)
        {
            var message = "Duplicate IDistributedLockDefinition.DistributedLockId: " + dulpicateIds.Key;
            throw new InvalidDistributedLockDefinitionException(message, dulpicateIds.First(), definitions);
        }

        var notValidCode = definitions
            .Where(d => !SqlCharValidator.IsValid(d.DistributedLockId, 6))
            .FirstOrDefault();

        if (notValidCode != null)
        {
            var message = notValidCode.GetType().Name + " has an invalid DistributedLockId. The id must be 6 characters in length and can include only non-unicode characters.";
            throw new InvalidDistributedLockDefinitionException(message, notValidCode, definitions);
        }
    }

    public IDistributedLockDefinition Get<TDefinition>()
        where TDefinition : IDistributedLockDefinition
    {
        return _definitionLookup.GetOrDefault(typeof(TDefinition));
    }

    public IEnumerable<IDistributedLockDefinition> GetAll()
    {
        return _definitionLookup.Values;
    }
}
