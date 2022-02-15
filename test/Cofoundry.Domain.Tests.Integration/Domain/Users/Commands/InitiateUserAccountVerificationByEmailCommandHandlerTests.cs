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
    public class InitiateUserAccountVerificationByEmailCommandHandlerTests
    {
        const string UNIQUE_PREFIX = "InitAccVerCHT ";

        private readonly DbDependentTestApplicationFactory _appFactory;

        public InitiateUserAccountVerificationByEmailCommandHandlerTests(
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
            var authenticatedTask = await AddUserAndInitiate(uniqueData, app);

            using (new AssertionScope())
            {
                authenticatedTask.Should().NotBeNull();
                authenticatedTask.CreateDate.Should().NotBeDefault();
                authenticatedTask.IPAddress.Address.Should().Be(TestIPAddresses.Localhost);
                authenticatedTask.CompletedDate.Should().BeNull();
                authenticatedTask.InvalidatedDate.Should().BeNull();
                authenticatedTask.AuthorizationCode.Should().NotBeEmpty();
                authenticatedTask.UserId.Should().BePositive();
                authenticatedTask.AuthorizedTaskId.Should().NotBeEmpty();
            }
        }

        [Fact]
        public async Task MaxAttemptsExceeded_Throws()
        {
            var uniqueData = UNIQUE_PREFIX + "MaxAttExceeded";
            var seedDate = new DateTime(1953, 01, 19);

            using var app = _appFactory.Create(s => s.Configure<UsersSettings>(s =>
            {
                s.AccountVerification.InitiationRateLimit.Quantity = 2;
                s.AccountVerification.InitiationRateLimit.Window = TimeSpan.FromHours(2);
            }));
            var contentRepository = app.Services.GetContentRepository();
            app.Mocks.MockDateTime(seedDate);
            var authenticatedTask = await AddUserAndInitiate(uniqueData + 1, app);
            var command = new InitiateUserAccountVerificationByEmailCommand()
            {
                UserId = authenticatedTask.UserId
            };

            app.Mocks.MockDateTime(seedDate.AddHours(1));
            await contentRepository.Users().AccountVerification().EmailFlow().InitiateAsync(command);

            using (new AssertionScope())
            {
                app.Mocks.MockDateTime(seedDate.AddHours(1).AddMinutes(59));
                await contentRepository
                    .Awaiting(r => r.Users().AccountVerification().EmailFlow().InitiateAsync(command))
                    .Should()
                    .ThrowAsync<ValidationErrorException>()
                    .WithErrorCode(UserValidationErrors.AccountVerification.Initiation.RateLimitExceeded.ErrorCode);

                app.Mocks.MockDateTime(seedDate.AddHours(2));
                await contentRepository
                    .Awaiting(r => r.Users().AccountVerification().EmailFlow().InitiateAsync(command))
                    .Should()
                    .NotThrowAsync();
            }
        }

        [Fact]
        public async Task SendsMail()
        {
            var userAreaCode = TestUserArea1.Code;
            var roleCode = TestUserArea1RoleB.Code;
            var uniqueData = UNIQUE_PREFIX + nameof(SendsMail);

            using var app = _appFactory.Create();
            var authenticatedTask = await AddUserAndInitiate(uniqueData, app, c =>
            {
                c.UserAreaCode = userAreaCode;
                c.RoleCode = roleCode;
                c.RoleId = null;
            });
            var token = MakeToken(authenticatedTask);

            app.Mocks
                .CountDispatchedMail(
                    authenticatedTask.User.Email,
                    "Please verify your account ",
                    "Test Site",
                    TestUserArea1.VerificationUrlBase,
                    token
                )
                .Should().Be(1);
        }

        [Fact]
        public async Task SendsMessage()
        {
            var uniqueData = UNIQUE_PREFIX + "SendMsg";

            using var app = _appFactory.Create();
            var authenticatedTask = await AddUserAndInitiate(uniqueData, app);
            var token = MakeToken(authenticatedTask);

            app.Mocks
                .CountMessagesPublished<UserAccountVerificationInitiatedMessage>(m =>
                {
                    return m.UserId == authenticatedTask.UserId
                        && m.UserAreaCode == TestUserArea1.Code
                        && m.AuthorizedTaskId == authenticatedTask.AuthorizedTaskId
                        && m.Token == token;
                })
                .Should().Be(1);
        }

        private static string MakeToken(AuthorizedTask authorizedTask)
        {
            var formatter = new AuthorizedTaskTokenFormatter();
            return formatter.Format(new AuthorizedTaskTokenParts()
            {
                AuthorizationCode = authorizedTask.AuthorizationCode,
                AuthorizedTaskId = authorizedTask.AuthorizedTaskId
            });
        }

        private static async Task<AuthorizedTask> AddUserAndInitiate(
            string uniqueData,
            DbDependentTestApplication app,
            Action<AddUserCommand> configration = null
            )
        {
            var contentRepository = app.Services.GetContentRepository();
            var dbContext = app.Services.GetRequiredService<CofoundryDbContext>();
            var addUserCommand = app.TestData.Users().CreateAddCommand(uniqueData);
            if (configration != null) configration(addUserCommand);

            var userId = await contentRepository
                .WithElevatedPermissions()
                .Users()
                .AddAsync(addUserCommand);

            await contentRepository
                .Users()
                .AccountVerification()
                .EmailFlow()
                .InitiateAsync(new InitiateUserAccountVerificationByEmailCommand()
                {
                    UserId = userId
                });

            var authorizedTask = await dbContext
                .AuthorizedTasks
                .AsNoTracking()
                .Include(u => u.User)
                .Include(u => u.IPAddress)
                .SingleAsync(u => u.UserId == addUserCommand.OutputUserId);

            return authorizedTask;
        }
    }
}
