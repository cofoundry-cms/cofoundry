using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;


namespace Cofoundry.Domain.Tests.Integration.Users.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class CompleteUserPasswordResetRequestCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "InitPWResetCHT ";
        const string _resetUri = "/auth/forgot-password";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public CompleteUserPasswordResetRequestCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task CreatesRequest()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CreatesRequest);

            using var app = _appFactory.Create();
            var resetRequest = await AddUserAndInitiateRequest(uniqueData, app);

            using (new AssertionScope())
            {
                resetRequest.Should().NotBeNull();
                resetRequest.CreateDate.Should().NotBeDefault();
                resetRequest.IPAddress.Should().Be(TestIPAddresses.Localhost);
                resetRequest.IsComplete.Should().BeFalse();
                resetRequest.Token.Should().NotBeEmpty();
                resetRequest.UserId.Should().BePositive();
                resetRequest.UserPasswordResetRequestId.Should().NotBeEmpty();
            }
        }

        [Theory]
        [InlineData(CofoundryAdminUserArea.AreaCode, SuperAdminRole.SuperAdminRoleCode)]
        [InlineData(TestUserArea1.Code, TestUserArea1RoleB.Code)]
        public async Task SendsMail(string userAreaCode, string roleCode)
        {
            var uniqueData = UNIQUE_PREFIX + nameof(SendsMail);

            using var app = _appFactory.Create();
            var resetRequest = await AddUserAndInitiateRequest(uniqueData, app, c =>
            {
                c.UserAreaCode = userAreaCode;
                c.RoleCode = roleCode;
                c.RoleId = null;
            });

            app.Mocks
                .CountDispatchedMail(
                    resetRequest.User.Email,
                    "request to reset the password for your account",
                    "Test Site",
                    _resetUri.ToString(),
                    resetRequest.UserPasswordResetRequestId.ToString("N"),
                    resetRequest.Token
                )
                .Should().Be(1);
        }


        [Fact]
        public async Task SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + "SendMsg";

            using var app = _appFactory.Create();
            var resetRequest = await AddUserAndInitiateRequest(uniqueData, app);

            app.Mocks
                .CountMessagesPublished<UserPasswordResetInitiatedMessage>(m =>
                {
                    return m.UserId == resetRequest.UserId
                        && m.UserAreaCode == TestUserArea1.Code
                        && m.UserPasswordResetRequestId == resetRequest.UserPasswordResetRequestId;
                })
                .Should().Be(1);
        }

        private static async Task<UserPasswordResetRequest> AddUserAndInitiateRequest(
            string uniqueData,
            DbDependentTestApplication app,
            Action<AddUserCommand> configration = null
            )
        {
            var contentRepository = app.Services.GetContentRepository();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var addUserCommand = app.TestData.Users().CreateAddCommand(uniqueData);
            if (configration != null) configration(addUserCommand);

            await contentRepository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(addUserCommand);

            await contentRepository
                .Users()
                .PasswordResetRequests()
                .InitiateAsync(new InitiateUserPasswordResetRequestCommand()
                {
                    UserAreaCode = addUserCommand.UserAreaCode,
                    Username = addUserCommand.Email,
                    ResetUrlBase = _resetUri
                });

            var resetRequest = await dbContext
                .UserPasswordResetRequests
                .AsNoTracking()
                .Include(u => u.User)
                .SingleAsync(u => u.UserId == addUserCommand.OutputUserId);

            return resetRequest;
        }
    }
}
