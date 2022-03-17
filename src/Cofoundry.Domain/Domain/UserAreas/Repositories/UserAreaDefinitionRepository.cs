namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class UserAreaDefinitionRepository : IUserAreaDefinitionRepository
{
    private readonly Dictionary<string, IUserAreaDefinition> _userAreas;
    private readonly Dictionary<string, UserAreaOptions> _options;
    private readonly IUserAreaDefinition _defaultUserArea;

    public UserAreaDefinitionRepository(
        IEnumerable<IUserAreaDefinition> userAreas,
        UsersSettings identitySettings
        )
    {
        DetectInvalidDefinitions(userAreas);
        _userAreas = userAreas.ToDictionary(k => k.UserAreaCode);
        _options = ConfigureOptions(userAreas, identitySettings);
        _defaultUserArea = userAreas
            .OrderByDescending(u => u.IsDefaultAuthScheme)
            .ThenByDescending(u => u is CofoundryAdminUserArea)
            .ThenBy(u => u.Name)
            .FirstOrDefault();

    }

    public IUserAreaDefinition GetByCode(string userAreaCode)
    {
        var area = _userAreas.GetOrDefault(userAreaCode);

        return area;
    }

    public IUserAreaDefinition GetRequiredByCode(string userAreaCode)
    {
        var definition = GetByCode(userAreaCode);
        ValidateDefinitionExists(definition, userAreaCode);

        return definition;
    }

    public IUserAreaDefinition GetRequired<TDefinition>()
        where TDefinition : IUserAreaDefinition
    {
        var definition = _userAreas
            .Select(p => p.Value)
            .FirstOrDefault(p => p is TDefinition);
        ValidateDefinitionExists(definition, typeof(TDefinition).Name);

        return definition;
    }

    private static void ValidateDefinitionExists(IUserAreaDefinition definition, string identifier)
    {
        if (definition == null)
        {
            throw new EntityNotFoundException<IUserAreaDefinition>($"IUserAreaDefinition '{identifier}' is not registered. but has been requested.", identifier);
        }
    }

    public IEnumerable<IUserAreaDefinition> GetAll()
    {
        return _userAreas.Select(a => a.Value);
    }

    public IUserAreaDefinition GetDefault()
    {
        return _defaultUserArea;
    }

    public UserAreaOptions GetOptionsByCode(string userAreaCode)
    {
        // Ensure exists
        var userArea = GetRequiredByCode(userAreaCode);
        var options = _options.GetOrDefault(userAreaCode);
        if (options == null)
        {
            throw new EntityInvalidOperationException($"{nameof(UserAreaOptions)} instance expected but was not found.");
        }

        return options;
    }

    private void DetectInvalidDefinitions(IEnumerable<IUserAreaDefinition> definitions)
    {
        var nullCode = definitions
            .Where(d => string.IsNullOrWhiteSpace(d.UserAreaCode))
            .FirstOrDefault();

        if (nullCode != null)
        {
            var message = nullCode.GetType().Name + " does not have a definition code specified.";
            throw new InvalidUserAreaDefinitionException(message, nullCode, definitions);
        }

        var duplicateCode = definitions
            .GroupBy(e => e.UserAreaCode, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .FirstOrDefault();

        if (duplicateCode != null)
        {
            var message = "Duplicate IUserAreaDefinition.UserAreaCode: " + duplicateCode.Key;
            throw new InvalidUserAreaDefinitionException(message, duplicateCode.First(), definitions);
        }

        var notValidCode = definitions
            .Where(d => !SqlCharValidator.IsValid(d.UserAreaCode, 3))
            .FirstOrDefault();

        if (notValidCode != null)
        {
            var message = notValidCode.GetType().Name + " has an invalid code. User area codes must be 3 characters and contain only non-unicode characters.";
            throw new InvalidUserAreaDefinitionException(message, notValidCode, definitions);
        }

        var nullName = definitions
            .Where(d => string.IsNullOrWhiteSpace(d.Name))
            .FirstOrDefault();

        if (nullName != null)
        {
            var message = nullName.GetType().Name + " does not have a name specified.";
            throw new InvalidUserAreaDefinitionException(message, nullName, definitions);
        }

        var nameTooLong = definitions
            .Where(d => d.Name.Length > 20)
            .FirstOrDefault();

        if (nameTooLong != null)
        {
            var message = nameTooLong.GetType().Name + " has a name that is more than 20 characters in length. All user area definition names must be 20 characters or less.";
            throw new InvalidUserAreaDefinitionException(message, nameTooLong, definitions);
        }

        var duplicateName = definitions
            .GroupBy(e => e.Name, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .FirstOrDefault();

        if (duplicateName != null)
        {
            var message = "Duplicate IUserAreaDefinition.Name: " + duplicateName.Key;
            throw new InvalidUserAreaDefinitionException(message, duplicateName.First(), definitions);
        }

        var defaultUserAreas = definitions
            .Where(d => d.IsDefaultAuthScheme);

        var invalidDefaultUserArea = defaultUserAreas.Skip(1).FirstOrDefault();
        if (invalidDefaultUserArea != null)
        {
            var message = $"More than one user area has {nameof(IUserAreaDefinition.IsDefaultAuthScheme)} defined. Only a single default user area can be defined. Duplicates: " + string.Join(", ", defaultUserAreas);
            throw new InvalidUserAreaDefinitionException(message, invalidDefaultUserArea, definitions);
        }
    }

    private static Dictionary<string, UserAreaOptions> ConfigureOptions(IEnumerable<IUserAreaDefinition> userAreas, UsersSettings identitySettings)
    {
        var result = new Dictionary<string, UserAreaOptions>();

        foreach (var userArea in userAreas)
        {
            var options = UserAreaOptions.CopyFrom(identitySettings);
            userArea.ConfigureOptions(options);
            result.Add(userArea.UserAreaCode, options);
        }

        return result;
    }
}
