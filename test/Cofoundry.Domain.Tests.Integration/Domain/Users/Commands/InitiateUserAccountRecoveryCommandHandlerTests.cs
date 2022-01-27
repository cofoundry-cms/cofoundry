using Cofoundry.Core.Validation;
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
    public class InitiateUserAccountRecoveryCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "InitPWResetCHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public InitiateUserAccountRecoveryCommandHandlerTests(
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
                resetRequest.CompletedDate.Should().BeNull();
                resetRequest.InvalidatedDate.Should().BeNull();
                resetRequest.AuthorizationCode.Should().NotBeEmpty();
                resetRequest.UserId.Should().BePositive();
                resetRequest.UserAccountRecoveryRequestId.Should().NotBeEmpty();
            }
        }

        [Fact]
        public async Task MaxAttemptsExceeded_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + "MaxAttExceeded";
            var seedDate = new DateTime(2022, 01, 19);

            using var app = _appFactory.Create(s => s.Configure<UsersSettings>(s =>
            {
                s.AccountRecovery.MaxAttempts = 2;
                s.AccountRecovery.MaxAttemptsWindow = TimeSpan.FromHours(2);
            }));
            var contentRepository = app.Services.GetContentRepository();
            app.Mocks.MockDateTime(seedDate);
            var request = await AddUserAndInitiateRequest(uniqueData + 1, app);
            var command = new InitiateUserAccountRecoveryCommand()
            {
                UserAreaCode = request.User.UserAreaCode,
                Username = request.User.Email
            };

            app.Mocks.MockDateTime(seedDate.AddHours(1));
            await contentRepository.Users().AccountRecovery().InitiateAsync(command);

            using (new AssertionScope())
            {
                app.Mocks.MockDateTime(seedDate.AddHours(1).AddMinutes(59));
                await contentRepository
                    .Awaiting(r => r.Users().AccountRecovery().InitiateAsync(command))
                    .Should()
                    .ThrowAsync<ValidationErrorException>()
                    .WithErrorCode(UserValidationErrors.AccountRecovery.Initiation.MaxAttemptsExceeded.ErrorCode);

                app.Mocks.MockDateTime(seedDate.AddHours(2));
                await contentRepository
                    .Awaiting(r => r.Users().AccountRecovery().InitiateAsync(command))
                    .Should()
                    .NotThrowAsync();
            }
        }

        [Theory]
        [InlineData(CofoundryAdminUserArea.Code, SuperAdminRole.SuperAdminRoleCode)]
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
            var token = MakeResetToken(resetRequest);

            app.Mocks
                .CountDispatchedMail(
                    resetRequest.User.Email,
                    "request to reset the password for your account",
                    "Test Site",
                    TestUserArea1.RecoveryUrlPath,
                    token
                )
                .Should().Be(1);
        }


        [Fact]
        public async Task SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + "SendMsg";

            using var app = _appFactory.Create();
            var resetRequest = await AddUserAndInitiateRequest(uniqueData, app);
            var token = MakeResetToken(resetRequest);

            app.Mocks
                .CountMessagesPublished<UserAccountRecoveryInitiatedMessage>(m =>
                {
                    return m.UserId == resetRequest.UserId
                        && m.UserAreaCode == TestUserArea1.Code
                        && m.UserAccountRecoveryRequestId == resetRequest.UserAccountRecoveryRequestId
                        && m.Token == token;
                })
                .Should().Be(1);
        }

        private static string MakeResetToken(UserAccountRecoveryRequest resetRequest)
        {
            var formatter = new UserAccountRecoveryTokenFormatter();
            return formatter.Format(new UserAccountRecoveryTokenParts()
            {
                AuthorizationCode = resetRequest.AuthorizationCode,
                UserAccountRecoveryRequestId = resetRequest.UserAccountRecoveryRequestId
            });
        }

        private static async Task<UserAccountRecoveryRequest> AddUserAndInitiateRequest(
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
                .AccountRecovery()
                .InitiateAsync(new InitiateUserAccountRecoveryCommand()
                {
                    UserAreaCode = addUserCommand.UserAreaCode,
                    Username = addUserCommand.Email
                });

            var resetRequest = await dbContext
                .UserAccountRecoveryRequests
                .AsNoTracking()
                .Include(u => u.User)
                .SingleAsync(u => u.UserId == addUserCommand.OutputUserId);

            return resetRequest;
        }
    }
}
