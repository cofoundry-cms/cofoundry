namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class RoleDefinitionRepository : IRoleDefinitionRepository
{
    private readonly Dictionary<string, IRoleDefinition> _roleDefinitions;

    public RoleDefinitionRepository(
        IEnumerable<IRoleDefinition> roleDefinitions
        )
    {
        DetectInvalidDefinitions(roleDefinitions);
        _roleDefinitions = roleDefinitions.ToDictionary(k => CreateLookupKey(k.UserAreaCode, k.RoleCode));
    }

    public IRoleDefinition GetByCode(string userAreaCode, string roleCode)
    {
        var key = CreateLookupKey(userAreaCode, roleCode);
        return _roleDefinitions.GetOrDefault(key);
    }

    public IRoleDefinition GetRequiredByCode(string userAreaCode, string roleCode)
    {
        var definition = GetByCode(userAreaCode, roleCode);

        if (definition == null)
        {
            throw new EntityNotFoundException<IRoleDefinition>($"IRoleDefinition '{roleCode}' in user area '{userAreaCode}' is not registered. but has been requested.", roleCode);
        }

        return definition;
    }

    public IRoleDefinition GetRequired<TDefinition>()
        where TDefinition : IRoleDefinition
    {
        var definition = _roleDefinitions
            .Select(p => p.Value)
            .FirstOrDefault(p => p is TDefinition);

        if (definition == null)
        {
            var identifier = typeof(TDefinition).Name;
            throw new EntityNotFoundException<IRoleDefinition>($"IRoleDefinition 'identifier' is not registered, but has been requested.", identifier);
        }

        return definition;
    }

    public IEnumerable<IRoleDefinition> GetAll()
    {
        return _roleDefinitions.Select(p => p.Value);
    }

    private void DetectInvalidDefinitions(IEnumerable<IRoleDefinition> definitions)
    {
        const string WHY_VALID_CODE_MESSAGE = "All role codes must be 3 characters and contain only non-unicode characters.";

        var nullRoleCode = definitions
            .Where(d => string.IsNullOrWhiteSpace(d.RoleCode))
            .FirstOrDefault();

        if (nullRoleCode != null)
        {
            var message = nullRoleCode.GetType().Name + " does not have a role code specified.";
            throw new InvalidRoleDefinitionException(message, nullRoleCode, definitions);
        }

        // The actual user area is validated during the registration process, but here we just need 
        // to check for a value to ensure we're validating the constraints properly.
        var nullUserAreaCode = definitions
            .Where(d => string.IsNullOrWhiteSpace(d.UserAreaCode))
            .FirstOrDefault();

        if (nullUserAreaCode != null)
        {
            var message = nullUserAreaCode.GetType().Name + " does not have a user area code specified.";
            throw new InvalidRoleDefinitionException(message, nullUserAreaCode, definitions);
        }

        var nullTitle = definitions
            .Where(d => string.IsNullOrWhiteSpace(d.Title))
            .FirstOrDefault();

        if (nullTitle != null)
        {
            var message = nullTitle.GetType().Name + " does not have a title specified.";
            throw new InvalidRoleDefinitionException(message, nullTitle, definitions);
        }

        var dulpicateCode = definitions
            .GroupBy(e => CreateLookupKey(e.UserAreaCode, e.RoleCode), StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .FirstOrDefault();

        if (dulpicateCode != null)
        {
            var example = dulpicateCode.First();
            var message = $"Duplicate role definitions encountered. { dulpicateCode.Count() } roles defined with the role code '{ example.RoleCode }' in user area { example.UserAreaCode }";
            throw new InvalidRoleDefinitionException(message, dulpicateCode.First(), definitions);
        }

        var dulpicateTitle = definitions
            .GroupBy(e => new { e.UserAreaCode, e.Title })
            .Where(g => g.Count() > 1)
            .FirstOrDefault();

        if (dulpicateTitle != null)
        {
            var message = "Duplicate IRoleDefinition.Title: " + dulpicateTitle.Key;
            throw new InvalidRoleDefinitionException(message, dulpicateTitle.First(), definitions);
        }

        var nameNot3Chars = definitions
            .Where(d => d.RoleCode.Length != 3)
            .FirstOrDefault();

        if (nameNot3Chars != null)
        {
            var message = nameNot3Chars.GetType().Name + " has a role code that is not 3 characters in length. " + WHY_VALID_CODE_MESSAGE;
            throw new InvalidRoleDefinitionException(message, nameNot3Chars, definitions);
        }

        var notValidCode = definitions
            .Where(d => !SqlCharValidator.IsValid(d.RoleCode, 3))
            .FirstOrDefault();

        if (notValidCode != null)
        {
            var message = notValidCode.GetType().Name + " has an invalid role code. " + WHY_VALID_CODE_MESSAGE;
            throw new InvalidRoleDefinitionException(message, notValidCode, definitions);
        }
    }

    private string CreateLookupKey(string userAreaCode, string code)
    {
        return userAreaCode + code;
    }
}
