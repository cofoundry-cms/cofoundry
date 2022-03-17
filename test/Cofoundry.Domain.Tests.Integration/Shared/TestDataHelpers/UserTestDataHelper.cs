namespace Cofoundry.Domain.Tests.Integration;

/// <summary>
/// Used to make it easier to create users in test fixtures.
/// </summary>
public class UserTestDataHelper
{
    private readonly IServiceProvider _serviceProvider;
    private readonly SeededEntities _seededEntities;

    public UserTestDataHelper(
        IServiceProvider serviceProvider,
        SeededEntities seededEntities
        )
    {
        _serviceProvider = serviceProvider;
        _seededEntities = seededEntities;
    }

    /// <summary>
    /// Adds a user that is is linked to TestUserArea1.
    /// </summary>
    /// <param name="uniqueData">
    /// Unique data to use in creating the email first name properties. 
    /// </param>
    /// <param name="configration">
    /// Optional additional configuration action to run before the
    /// command is executed.
    /// </param>
    /// <returns>The UserId of the newly created user.</returns>
    public async Task<int> AddAsync(
        string uniqueData,
        Action<AddUserCommand> configration
        )
    {
        var command = CreateAddCommand(uniqueData);

        if (configration != null)
        {
            configration(command);
        }

        using var scope = _serviceProvider.CreateScope();
        var contentRepository = scope.ServiceProvider.GetRequiredService<IAdvancedContentRepository>();

        return await contentRepository
            .WithElevatedPermissions()
            .Users()
            .AddAsync(command);
    }

    /// <summary>
    /// Adds a user that is is linked to TestUserArea1.
    /// </summary>
    /// <param name="uniqueData">
    /// Unique data to use in creating the email first name properties. 
    /// </param>
    /// <param name="domainUniqueData">
    /// Optional unique data to use in creating the email domain and last name properties. 
    /// </param>
    /// <param name="configration">
    /// Optional additional configuration action to run before the
    /// command is executed.
    /// </param>
    /// <returns>The UserId of the newly created user.</returns>
    public async Task<int> AddAsync(
        string uniqueData,
        string domainUniqueData = null,
        Action<AddUserCommand> configration = null
        )
    {
        var command = CreateAddCommand(uniqueData, domainUniqueData);

        if (configration != null)
        {
            configration(command);
        }

        using var scope = _serviceProvider.CreateScope();
        var contentRepository = scope.ServiceProvider.GetRequiredService<IAdvancedContentRepository>();

        return await contentRepository
            .WithElevatedPermissions()
            .Users()
            .AddAsync(command);
    }

    /// <summary>
    /// Creates a valid <see cref="AddUserCommand"/> that is linked to
    /// TestUserArea1.
    /// </summary>
    /// <param name="uniqueData">
    /// Unique data to use in creating the email first name properties. 
    /// </param>
    /// <param name="domainUniqueData">
    /// Optional unique data to use in creating the email domain and last name properties. 
    /// </param>
    public AddUserCommand CreateAddCommand(string uniqueData, string domainUniqueData = null)
    {
        var domain = "example.com";

        if (domainUniqueData != null)
        {
            domain = domainUniqueData + "." + domain;
        }

        var command = new AddUserCommand()
        {
            Email = uniqueData + "@" + domain,
            Password = "aN3xamp!eValu",
            FirstName = uniqueData,
            LastName = domainUniqueData ?? "User",
            UserAreaCode = _seededEntities.TestUserArea1.UserAreaCode,
            RoleId = _seededEntities.TestUserArea1.RoleA.RoleId
        };

        return command;
    }
}
