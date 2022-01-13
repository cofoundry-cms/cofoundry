using Cofoundry.Core.Web;
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
        const string UNIQUE_PREFIX = "CompPWResetCHT ";
        const string _resetUri = "/auth/forgot-password";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public CompleteUserPasswordResetRequestCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task CompletesReset()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CompletesReset);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var resetRequest = await AddUserAndInitiateRequest(uniqueData, app);

            await contentRepository
                .Users()
                .PasswordResetRequests()
                .CompleteAsync(new CompleteUserPasswordResetRequestCommand()
                {
                    NewPassword = "Re-33-set.",
                    SendNotification = false,
                    Token = resetRequest.Token,
                    UserAreaCode = resetRequest.User.UserAreaCode,
                    UserPasswordResetRequestId = resetRequest.UserPasswordResetRequestId
                });

            var completedResetRequest = await GetResetRequest(app, resetRequest.UserId);

            using (new AssertionScope())
            {
                completedResetRequest.Should().NotBeNull();
                completedResetRequest.IsComplete.Should().BeTrue();
                completedResetRequest.User.RequirePasswordChange.Should().BeFalse();
                completedResetRequest.User.Password.Should().NotBe(resetRequest.User.Password);
                completedResetRequest.User.LastPasswordChangeDate.Should().NotBe(resetRequest.User.LastPasswordChangeDate);
                completedResetRequest.User.SecurityStamp.Should().NotBeNull().And.NotBe(resetRequest.User.SecurityStamp);
            }
        }

        [Theory]
        [InlineData(CofoundryAdminUserArea.AreaCode, SuperAdminRole.SuperAdminRoleCode)]
        [InlineData(TestUserArea1.Code, TestUserArea1RoleB.Code)]
        public async Task SendsMail(string userAreaCode, string roleCode)
        {
            var uniqueData = UNIQUE_PREFIX + nameof(SendsMail);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var siteUrlResolver = app.Services.GetRequiredService<ISiteUrlResolver>();
            var loginUrl = siteUrlResolver.MakeAbsolute(app.SeededEntities.TestUserArea1.Definition.LoginPath);
            var resetRequest = await AddUserAndInitiateRequest(uniqueData, app, c =>
            {
                c.UserAreaCode = userAreaCode;
                c.RoleCode = roleCode;
                c.RoleId = null;
            });

            await contentRepository
                .Users()
                .PasswordResetRequests()
                .CompleteAsync(new CompleteUserPasswordResetRequestCommand()
                {
                    NewPassword = "Re-33-set.",
                    SendNotification = false,
                    Token = resetRequest.Token,
                    UserAreaCode = resetRequest.User.UserAreaCode,
                    UserPasswordResetRequestId = resetRequest.UserPasswordResetRequestId
                });

            app.Mocks
                .CountDispatchedMail(
                    resetRequest.User.Email,
                    "password",
                    "Test Site",
                    "has been changed",
                    "username registered for this account is: " + resetRequest.User.Username,
                    loginUrl
                )
                .Should().Be(1);
        }


        [Fact]
        public async Task SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + "SendMsg";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var resetRequest = await AddUserAndInitiateRequest(uniqueData, app);

            await contentRepository
                .Users()
                .PasswordResetRequests()
                .CompleteAsync(new CompleteUserPasswordResetRequestCommand()
                {
                    NewPassword = "Re-33-set.",
                    SendNotification = false,
                    Token = resetRequest.Token,
                    UserAreaCode = resetRequest.User.UserAreaCode,
                    UserPasswordResetRequestId = resetRequest.UserPasswordResetRequestId
                });

            app.Mocks
                .CountMessagesPublished<UserPasswordResetCompletedMessage>(m =>
                {
                    return m.UserId == resetRequest.UserId
                        && m.UserAreaCode == TestUserArea1.Code
                        && m.UserPasswordResetRequestId == resetRequest.UserPasswordResetRequestId;
                })
                .Should().Be(1);

            app.Mocks
                .CountMessagesPublished<UserPasswordUpdatedMessage>(m =>
                {
                    return m.UserId == resetRequest.UserId
                        && m.UserAreaCode == TestUserArea1.Code;
                })
                .Should().Be(1);

            app.Mocks
                .CountMessagesPublished<UserSecurityStampUpdatedMessage>(m =>
                {
                    return m.UserId == resetRequest.UserId
                        && m.UserAreaCode == TestUserArea1.Code;
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

            var resetRequest = await GetResetRequest(app, addUserCommand.OutputUserId);

            return resetRequest;
        }

        private static async Task<UserPasswordResetRequest> GetResetRequest(
            DbDependentTestApplication app,
            int userId
            )
        {
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            return await dbContext
                .UserPasswordResetRequests
                .AsNoTracking()
                .Include(u => u.User)
                .SingleAsync(u => u.UserId == userId);
        }
    }
}
