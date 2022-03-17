using Cofoundry.Core.Validation;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared.SeedData;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Tests.Integration.Users.Commands;

[Collection(nameof(DbDependentFixtureCollection))]
public class UpdateUserCommandHandlerTests
{
    private const string UNIQUE_PREFIX = "UpdUsrCHT-";
    private const string PASSWORD = "neverbr3@kthechange";
    private static string EMAIL_DOMAIN = $"@{UNIQUE_PREFIX}.example.com";
    private readonly DbDependentTestApplicationFactory _appFactory;

    public UpdateUserCommandHandlerTests(
        DbDependentTestApplicationFactory appFactory
        )
    {
        _appFactory = appFactory;
    }

    [Fact]
    public async Task CanUpdateBasicProperties()
    {
        var alternateDomain = $"{UNIQUE_PREFIX}2.example.com";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var userArea = app.SeededEntities.TestUserArea1;

        var addCommand = new AddUserCommand()
        {
            Email = "reginald.dwight" + EMAIL_DOMAIN,
            Password = PASSWORD,
            FirstName = "Reginald",
            LastName = "Dwight",
            DisplayName = "Reggie",
            RoleCode = userArea.RoleA.RoleCode,
            UserAreaCode = userArea.UserAreaCode,
            RequirePasswordChange = false
        };

        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var originalUserState = await GetUserByIdAsync(app, userId);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.Email = "E.john@" + alternateDomain;
        updateCommand.FirstName = "Elton";
        updateCommand.LastName = "John";
        updateCommand.DisplayName = "Rocketman";

        await contentRepository
            .Users()
            .UpdateAsync(updateCommand);

        var user = await GetUserByIdAsync(app, userId);

        var normalizedDomain = alternateDomain.ToLowerInvariant();
        var normalizedEmail = "E.john@" + normalizedDomain;
        var lowerEmail = normalizedEmail.ToLowerInvariant();

        using (new AssertionScope())
        {
            user.Should().NotBeNull();
            user.FirstName.Should().Be(updateCommand.FirstName);
            user.LastName.Should().Be(updateCommand.LastName);
            user.DisplayName.Should().Be(updateCommand.DisplayName);
            user.Email.Should().Be(normalizedEmail);
            user.UniqueEmail.Should().Be(lowerEmail);
            user.Username.Should().Be(normalizedEmail);
            user.UniqueUsername.Should().Be(lowerEmail);
            user.AccountVerifiedDate.Should().BeNull();
            user.DeactivatedDate.Should().BeNull();
            user.RequirePasswordChange.Should().BeFalse();
            user.EmailDomain.Name.Should().Be(normalizedDomain);
            user.SecurityStamp.Should().NotBeNull().And.NotBe(originalUserState.SecurityStamp);
            user.AccountVerifiedDate.Should().BeNull();
        }
    }

    [Fact]
    public async Task WhenUsernameAsDisplayName_DisplayNameUpdated()
    {
        var uniqueData = UNIQUE_PREFIX + "UnAsDN_DNUpd";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var userAreaCode = UserAreaWithoutEmailAsUsername.Code;
        var roleId = await app.TestData.Roles().AddAsync(uniqueData, userAreaCode);

        var addCommand = new AddUserCommand()
        {
            Email = uniqueData + "@example.com",
            Username = uniqueData,
            Password = PASSWORD,
            RoleId = roleId,
            UserAreaCode = userAreaCode,
            RequirePasswordChange = false
        };

        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.Username = uniqueData + "1";

        await contentRepository
            .Users()
            .UpdateAsync(updateCommand);

        var user = await GetUserByIdAsync(app, userId);

        using (new AssertionScope())
        {
            user.Should().NotBeNull();
            user.DisplayName.Should().Be(updateCommand.DisplayName);
        }
    }

    [Fact]
    public async Task CanUnsetData()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanUnsetData);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var userAreaCode = UserAreaWithoutPasswordSignIn.Code;
        var roleId = await app.TestData.Roles().AddAsync(uniqueData, userAreaCode);

        var addCommand = new AddUserCommand()
        {
            Email = $"e.razor" + EMAIL_DOMAIN,
            Username = uniqueData,
            FirstName = "John",
            LastName = "Kruger",
            DisplayName = "Eraser",
            RoleId = roleId,
            UserAreaCode = userAreaCode
        };

        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var originalUserState = await GetUserByIdAsync(app, userId);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.Email = null;
        updateCommand.FirstName = null;
        updateCommand.LastName = null;

        await contentRepository
            .Users()
            .UpdateAsync(updateCommand);

        var user = await GetUserByIdAsync(app, userId);

        using (new AssertionScope())
        {
            user.Should().NotBeNull();
            user.FirstName.Should().BeNull();
            user.LastName.Should().BeNull();
            user.DisplayName.Should().BeNull();
            user.Email.Should().BeNull();
            user.UniqueEmail.Should().BeNull();
            user.EmailDomainId.Should().BeNull();
            user.Username.Should().Be(addCommand.Username);
            user.UniqueUsername.Should().Be(addCommand.Username.ToLowerInvariant());
            user.SecurityStamp.Should().NotBeNull().And.NotBe(originalUserState.SecurityStamp);
        }
    }

    [Fact]
    public async Task CanChangeRoleById()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanChangeRoleById);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var userArea = app.SeededEntities.TestUserArea1;
        var roleId = await app.TestData.Roles().AddAsync(uniqueData, userArea.UserAreaCode);

        var addCommand = new AddUserCommand()
        {
            Email = uniqueData + EMAIL_DOMAIN,
            Password = PASSWORD,
            RoleCode = userArea.RoleA.RoleCode,
            UserAreaCode = userArea.UserAreaCode
        };

        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.RoleCode = null;
        updateCommand.RoleId = roleId;

        await contentRepository
            .Users()
            .UpdateAsync(updateCommand);

        var user = await GetUserByIdAsync(app, userId);

        using (new AssertionScope())
        {
            user.Should().NotBeNull();
            user.RoleId.Should().Be(updateCommand.RoleId);
        }
    }

    [Fact]
    public async Task CanChangeRoleByCode()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanChangeRoleByCode);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var userArea = app.SeededEntities.TestUserArea1;

        var addCommand = new AddUserCommand()
        {
            Email = uniqueData + EMAIL_DOMAIN,
            Password = PASSWORD,
            RoleCode = userArea.RoleA.RoleCode,
            UserAreaCode = userArea.UserAreaCode
        };

        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.RoleCode = userArea.RoleB.RoleCode;

        await contentRepository
            .Users()
            .UpdateAsync(updateCommand);

        var user = await GetUserByIdAsync(app, userId);

        using (new AssertionScope())
        {
            user.Should().NotBeNull();
            user.Role.RoleCode.Should().Be(updateCommand.RoleCode);
        }
    }

    [Fact]
    public async Task CantChangeUserAreaByRoleId()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CantChangeUserAreaByRoleId);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var userArea1 = app.SeededEntities.TestUserArea1;
        var roleId = await app.TestData.Roles().AddAsync(uniqueData, app.SeededEntities.TestUserArea2.UserAreaCode);

        var addCommand = new AddUserCommand()
        {
            Email = uniqueData + EMAIL_DOMAIN,
            Password = PASSWORD,
            RoleCode = userArea1.RoleA.RoleCode,
            UserAreaCode = userArea1.UserAreaCode
        };

        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.RoleCode = null;
        updateCommand.RoleId = roleId;

        await contentRepository
            .Awaiting(r => r.Users().UpdateAsync(updateCommand))
            .Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("*user area*");
    }

    [Fact]
    public async Task CantChangeUserAreaByRoleCode()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CantChangeUserAreaByRoleCode);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var userArea1 = app.SeededEntities.TestUserArea1;
        var userArea2 = app.SeededEntities.TestUserArea2;

        var addCommand = new AddUserCommand()
        {
            Email = uniqueData + EMAIL_DOMAIN,
            Password = PASSWORD,
            RoleCode = userArea1.RoleA.RoleCode,
            UserAreaCode = userArea1.UserAreaCode
        };

        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.RoleCode = userArea2.RoleB.RoleCode;

        await contentRepository
            .Awaiting(r => r.Users().UpdateAsync(updateCommand))
            .Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMessage("*user area*");
    }

    [Fact]
    public async Task CanRequirePasswordChange()
    {
        var uniqueData = UNIQUE_PREFIX + "ReqPwChng";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();

        var addCommand = app.TestData.Users().CreateAddCommand(uniqueData);
        addCommand.RequirePasswordChange = false;
        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var originalUser = await GetUserByIdAsync(app, userId);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.RequirePasswordChange = true;

        await contentRepository
            .Users()
            .UpdateAsync(updateCommand);

        var user = await GetUserByIdAsync(app, userId);

        using (new AssertionScope())
        {
            user.Should().NotBeNull();
            user.RequirePasswordChange.Should().BeTrue();
            user.SecurityStamp.Should().NotBe(originalUser.SecurityStamp);
            app.Mocks
                .CountMessagesPublished<UserSecurityStampUpdatedMessage>()
                .Should().Be(1);
        }
    }


    [Fact]
    public async Task CanVerifyAccount()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanVerifyAccount);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var addCommand = app.TestData.Users().CreateAddCommand(uniqueData);
        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.IsAccountVerified = true;

        await contentRepository
            .Users()
            .UpdateAsync(updateCommand);

        var user = await GetUserByIdAsync(app, userId);

        using (new AssertionScope())
        {
            user.Should().NotBeNull();
            user.AccountVerifiedDate.Should().HaveValue();
            app.Mocks
                .CountMessagesPublished<UserAccountVerificationStatusUpdatedMessage>(m => m.UserId == addCommand.OutputUserId && m.UserAreaCode == addCommand.UserAreaCode && m.IsVerified)
                .Should().Be(1);
        }
    }

    [Fact]
    public async Task CanUnVerifyAccount()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanUnVerifyAccount);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var addCommand = app.TestData.Users().CreateAddCommand(uniqueData);
        addCommand.IsAccountVerified = true;
        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.IsAccountVerified = false;

        await contentRepository
            .Users()
            .UpdateAsync(updateCommand);

        var user = await GetUserByIdAsync(app, userId);

        using (new AssertionScope())
        {
            user.Should().NotBeNull();
            user.AccountVerifiedDate.Should().BeNull();
            app.Mocks
                .CountMessagesPublished<UserAccountVerificationStatusUpdatedMessage>(m => m.UserId == addCommand.OutputUserId && m.UserAreaCode == addCommand.UserAreaCode && !m.IsVerified)
                .Should().Be(1);
        }
    }

    [Fact]
    public async Task CanActivate()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanActivate);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var addCommand = app.TestData.Users().CreateAddCommand(uniqueData);
        addCommand.IsActive = false;
        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.IsActive = true;

        await contentRepository
            .Users()
            .UpdateAsync(updateCommand);

        var user = await GetUserByIdAsync(app, userId);

        using (new AssertionScope())
        {
            user.Should().NotBeNull();
            user.DeactivatedDate.Should().BeNull();
            app.Mocks
                .CountMessagesPublished<UserActivationStatusUpdatedMessage>(m => m.UserId == addCommand.OutputUserId && m.UserAreaCode == addCommand.UserAreaCode && m.IsActive)
                .Should().Be(1);
            app.Mocks
                .CountMessagesPublished<UserSecurityStampUpdatedMessage>()
                .Should().Be(0);
        }
    }

    [Fact]
    public async Task CanDeactivate()
    {
        var uniqueData = UNIQUE_PREFIX + nameof(CanDeactivate);

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        var addCommand = app.TestData.Users().CreateAddCommand(uniqueData);
        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var originalUserState = await GetUserByIdAsync(app, userId);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.IsActive = false;

        await contentRepository
            .Users()
            .UpdateAsync(updateCommand);

        var user = await GetUserByIdAsync(app, userId);

        using (new AssertionScope())
        {
            user.Should().NotBeNull();
            user.SecurityStamp.Should().NotBeNull().And.NotBe(originalUserState.SecurityStamp);
            user.DeactivatedDate.Should().NotBeNull();
            app.Mocks
                .CountMessagesPublished<UserActivationStatusUpdatedMessage>(m => m.UserId == addCommand.OutputUserId && m.UserAreaCode == addCommand.UserAreaCode && !m.IsActive)
                .Should().Be(1);
            app.Mocks
                .CountMessagesPublished<UserSecurityStampUpdatedMessage>(m => m.UserId == addCommand.OutputUserId && m.UserAreaCode == addCommand.UserAreaCode)
                .Should().Be(1);
        }
    }

    [Fact]
    public async Task WhenUsernameNotUnique_Throws()
    {
        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var userArea = app.SeededEntities.TestUserArea1;

        await contentRepository
            .Users()
            .AddAsync(new AddUserCommand()
            {
                Email = $"djones" + EMAIL_DOMAIN,
                Password = PASSWORD,
                RoleCode = userArea.RoleA.RoleCode,
                UserAreaCode = userArea.UserAreaCode
            });

        var addCommand = new AddUserCommand()
        {
            Email = $"dbowie" + EMAIL_DOMAIN,
            Password = PASSWORD,
            RoleCode = userArea.RoleA.RoleCode,
            UserAreaCode = userArea.UserAreaCode
        };

        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.Email = $"djones" + EMAIL_DOMAIN;

        await contentRepository
            .Awaiting(r => r.Users().UpdateAsync(updateCommand))
            .Should()
            .ThrowAsync<ValidationErrorException>()
            .WithMemberNames(nameof(updateCommand.Email))
            .WithMessage("*already registered*");
    }

    [Fact]
    public async Task WhenSystemUser_Throws()
    {
        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var userContextService = app.Services.GetService<IUserContextService>();

        var systemUser = await userContextService.GetSystemUserContextAsync();

        var command = new UpdateUserCommand()
        {
            UserId = systemUser.UserId.Value,
            RoleId = systemUser.RoleId,
            FirstName = "Trolo",
            LastName = "Lo"
        };

        await contentRepository
            .Awaiting(r => r.Users().UpdateAsync(command))
            .Should()
            .ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task WhenNameUpdated_SendsMessage()
    {
        var uniqueData = UNIQUE_PREFIX + "NameUpd_SM";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var addCommand = app.TestData.Users().CreateAddCommand(uniqueData);

        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.FirstName = uniqueData + "_X";

        await contentRepository
            .Users()
            .UpdateAsync(updateCommand);

        using (new AssertionScope())
        {
            app.Mocks
                .CountMessagesPublished<UserUpdatedMessage>(m => m.UserId == addCommand.OutputUserId && m.UserAreaCode == addCommand.UserAreaCode)
                .Should().Be(1);

            app.Mocks
                .CountMessagesPublished<UserUsernameUpdatedMessage>()
                .Should().Be(0);

            app.Mocks
                .CountMessagesPublished<UserEmailUpdatedMessage>()
                .Should().Be(0);

            app.Mocks
                .CountMessagesPublished<UserSecurityStampUpdatedMessage>()
                .Should().Be(0);

            app.Mocks
                .CountMessagesPublished<UserAccountVerificationStatusUpdatedMessage>()
                .Should().Be(0);

            app.Mocks
                .CountMessagesPublished<UserAccountVerificationStatusUpdatedMessage>()
                .Should().Be(0);
        }
    }

    [Fact]
    public async Task WhenEmailUpdated_SendsMessage()
    {
        var uniqueData = UNIQUE_PREFIX + "EmailUpd_SM";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var addCommand = app.TestData.Users().CreateAddCommand(uniqueData);

        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.Email = "x." + addCommand.Email;

        await contentRepository
            .Users()
            .UpdateAsync(updateCommand);

        using (new AssertionScope())
        {
            app.Mocks
                .CountMessagesPublished<UserUpdatedMessage>(m => m.UserId == addCommand.OutputUserId && m.UserAreaCode == addCommand.UserAreaCode)
                .Should().Be(1);

            app.Mocks
                .CountMessagesPublished<UserEmailUpdatedMessage>(m => m.UserId == addCommand.OutputUserId && m.UserAreaCode == addCommand.UserAreaCode)
                .Should().Be(1);

            app.Mocks
                .CountMessagesPublished<UserUsernameUpdatedMessage>(m => m.UserId == addCommand.OutputUserId && m.UserAreaCode == addCommand.UserAreaCode)
                .Should().Be(1);

            app.Mocks
                .CountMessagesPublished<UserSecurityStampUpdatedMessage>(m => m.UserId == addCommand.OutputUserId && m.UserAreaCode == addCommand.UserAreaCode)
                .Should().Be(1);
        }
    }

    [Fact]
    public async Task WhenUsernameUpdated_SendsMessage()
    {
        var uniqueData = UNIQUE_PREFIX + "UsernameUpd_SM";

        using var app = _appFactory.Create();
        var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
        var userAreaCode = UserAreaWithoutPasswordSignIn.Code;
        var roleId = await app.TestData.Roles().AddAsync(uniqueData, userAreaCode);
        var addCommand = new AddUserCommand()
        {
            Username = uniqueData,
            RoleId = roleId,
            UserAreaCode = userAreaCode
        };

        var userId = await contentRepository
            .Users()
            .AddAsync(addCommand);

        var updateCommand = MapUpdateCommand(addCommand);
        updateCommand.Username = uniqueData + "_X";

        await contentRepository
            .Users()
            .UpdateAsync(updateCommand);

        using (new AssertionScope())
        {
            app.Mocks
                .CountMessagesPublished<UserUpdatedMessage>(m => m.UserId == addCommand.OutputUserId && m.UserAreaCode == addCommand.UserAreaCode)
                .Should().Be(1);

            app.Mocks
                .CountMessagesPublished<UserEmailUpdatedMessage>(m => m.UserId == addCommand.OutputUserId && m.UserAreaCode == addCommand.UserAreaCode)
                .Should().Be(0);

            app.Mocks
                .CountMessagesPublished<UserUsernameUpdatedMessage>(m => m.UserId == addCommand.OutputUserId && m.UserAreaCode == addCommand.UserAreaCode)
                .Should().Be(1);

            app.Mocks
                .CountMessagesPublished<UserSecurityStampUpdatedMessage>(m => m.UserId == addCommand.OutputUserId && m.UserAreaCode == addCommand.UserAreaCode)
                .Should().Be(1);
        }
    }

    private UpdateUserCommand MapUpdateCommand(AddUserCommand command)
    {
        return new UpdateUserCommand()
        {
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            RequirePasswordChange = command.RequirePasswordChange,
            RoleId = command.RoleId,
            RoleCode = command.RoleCode,
            UserId = command.OutputUserId,
            Username = command.Username,
            IsActive = command.IsActive,
            IsAccountVerified = command.IsAccountVerified
        };
    }

    private async Task<User> GetUserByIdAsync(DbDependentTestApplication app, int userId)
    {
        var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

        return await dbContext
            .Users
            .AsNoTracking()
            .Include(u => u.EmailDomain)
            .Include(u => u.Role)
            .FilterById(userId)
            .SingleOrDefaultAsync();
    }
}
