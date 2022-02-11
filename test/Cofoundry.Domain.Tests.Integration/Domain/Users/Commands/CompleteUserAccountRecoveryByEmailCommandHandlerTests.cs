using Cofoundry.Core.Web;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Tests.Shared;
using Cofoundry.Domain.Tests.Shared.Assertions;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;


namespace Cofoundry.Domain.Tests.Integration.Users.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class CompleteUserAccountRecoveryByEmailCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "CompAccRecCHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public CompleteUserAccountRecoveryByEmailCommandHandlerTests(
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
            var token = MakeResetToken(resetRequest);

            await contentRepository
                .Users()
                .AccountRecovery()
                .CompleteAsync(new CompleteUserAccountRecoveryByEmailCommand()
                {
                    NewPassword = "Re-33-set.",
                    Token = token,
                    UserAreaCode = resetRequest.User.UserAreaCode,
                });

            var completedResetRequest = await GetResetRequest(app, resetRequest.UserId);

            using (new AssertionScope())
            {
                completedResetRequest.Should().NotBeNull();
                completedResetRequest.CompletedDate.Should().NotBeNull();
                completedResetRequest.User.RequirePasswordChange.Should().BeFalse();
                completedResetRequest.User.Password.Should().NotBe(resetRequest.User.Password);
                completedResetRequest.User.LastPasswordChangeDate.Should().NotBe(resetRequest.User.LastPasswordChangeDate);
                completedResetRequest.User.SecurityStamp.Should().NotBeNull().And.NotBe(resetRequest.User.SecurityStamp);
            }
        }

        [Theory]
        [InlineData(CofoundryAdminUserArea.Code, SuperAdminRole.Code, "/admin")]
        [InlineData(TestUserArea1.Code, TestUserArea1RoleB.Code, TestUserArea1.SignInPathSetting)]
        public async Task SendsMail(string userAreaCode, string roleCode, string signInPath)
        {
            var uniqueData = UNIQUE_PREFIX + nameof(SendsMail);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var siteUrlResolver = app.Services.GetRequiredService<ISiteUrlResolver>();

            var resetRequest = await AddUserAndInitiateRequest(uniqueData, app, c =>
            {
                c.UserAreaCode = userAreaCode;
                c.RoleCode = roleCode;
                c.RoleId = null;
            });
            var token = MakeResetToken(resetRequest);
            var signInUrl = siteUrlResolver.MakeAbsolute(signInPath);

            await contentRepository
                .Users()
                .AccountRecovery()
                .CompleteAsync(new CompleteUserAccountRecoveryByEmailCommand()
                {
                    NewPassword = "Re-33-set.",
                    Token = token,
                    UserAreaCode = resetRequest.User.UserAreaCode,
                });

            app.Mocks
                .CountDispatchedMail(
                    resetRequest.User.Email,
                    "password",
                    "Test Site",
                    "has been changed",
                    "account is " + resetRequest.User.Username,
                    signInUrl
                )
                .Should().Be(1);
        }

        [Fact]
        public async Task InvalidatesOldResetTokens()
        {
            var uniqueData = UNIQUE_PREFIX + nameof(CompletesReset);

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var resetRequest1 = await AddUserAndInitiateRequest(uniqueData + 1, app);
            var token = MakeResetToken(resetRequest1);
            var resetRequest2 = await AddUserAndInitiateRequest(uniqueData + 2, app);

            await contentRepository
                .Users()
                .AccountRecovery()
                .InitiateAsync(new InitiateUserAccountRecoveryByEmailCommand()
                {
                    UserAreaCode = resetRequest1.User.UserAreaCode,
                    Username = resetRequest1.User.Email
                });

            await contentRepository
                .Users()
                .AccountRecovery()
                .CompleteAsync(new CompleteUserAccountRecoveryByEmailCommand()
                {
                    NewPassword = "Re-33-set.",
                    Token = token,
                    UserAreaCode = resetRequest1.User.UserAreaCode,
                });

            var resetRequests = await dbContext
                .AuthorizedTasks
                .AsNoTracking()
                .Where(u => u.UserId == resetRequest1.UserId || u.UserId == resetRequest2.UserId)
                .ToListAsync();

            var resetRequest1Result = resetRequests.SingleOrDefault(r => r.AuthorizedTaskId == resetRequest1.AuthorizedTaskId);
            var resetRequest2Result = resetRequests.SingleOrDefault(r => r.AuthorizedTaskId == resetRequest2.AuthorizedTaskId);
            var resetRequest3Result = resetRequests.SingleOrDefault(r => r.UserId == resetRequest1.UserId && r.AuthorizedTaskId != resetRequest1.AuthorizedTaskId);

            using (new AssertionScope())
            {
                resetRequests.Should().HaveCount(3);
                resetRequest1Result.Should().NotBeNull();
                resetRequest1Result.CompletedDate.Should().NotBeNull();
                resetRequest1Result.InvalidatedDate.Should().BeNull();

                resetRequest2Result.Should().NotBeNull();
                resetRequest2Result.CompletedDate.Should().BeNull();
                resetRequest2Result.InvalidatedDate.Should().BeNull();

                resetRequest3Result.Should().NotBeNull();
                resetRequest3Result.CompletedDate.Should().BeNull();
                resetRequest3Result.InvalidatedDate.Should().NotBeNull();
            }
        }

        [Fact]
        public async Task SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + "SendMsg";

            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepository();
            var resetRequest = await AddUserAndInitiateRequest(uniqueData, app);
            var token = MakeResetToken(resetRequest);

            await contentRepository
                .Users()
                .AccountRecovery()
                .CompleteAsync(new CompleteUserAccountRecoveryByEmailCommand()
                {
                    NewPassword = "Re-33-set.",
                    Token = token,
                    UserAreaCode = resetRequest.User.UserAreaCode
                });

            app.Mocks
                .CountMessagesPublished<UserAccountRecoveryCompletedMessage>(m =>
                {
                    return m.UserId == resetRequest.UserId
                        && m.UserAreaCode == TestUserArea1.Code
                        && m.AuthorizedTaskId == resetRequest.AuthorizedTaskId;
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

        private static string MakeResetToken(AuthorizedTask authorizedTask)
        {
            var formatter = new AuthorizedTaskTokenFormatter();
            return formatter.Format(new AuthorizedTaskTokenParts()
            {
                AuthorizationCode = authorizedTask.AuthorizationCode,
                AuthorizedTaskId = authorizedTask.AuthorizedTaskId
            });
        }

        private static async Task<AuthorizedTask> AddUserAndInitiateRequest(
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
                .AccountRecovery()
                .InitiateAsync(new InitiateUserAccountRecoveryByEmailCommand()
                {
                    UserAreaCode = addUserCommand.UserAreaCode,
                    Username = addUserCommand.Email
                });

            var resetRequest = await GetResetRequest(app, addUserCommand.OutputUserId);

            return resetRequest;
        }

        private static async Task<AuthorizedTask> GetResetRequest(
            DbDependentTestApplication app,
            int userId
            )
        {
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();

            var authorizedTask = await dbContext
                .AuthorizedTasks
                .AsNoTracking()
                .Include(u => u.User)
                .SingleAsync(u => u.UserId == userId);

            return authorizedTask;
        }
    }
}
