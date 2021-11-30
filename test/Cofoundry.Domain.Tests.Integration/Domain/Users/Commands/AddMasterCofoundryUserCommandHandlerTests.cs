using Cofoundry.Domain.Tests.Shared.Mocks;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Integration.Users.Commands
{
    [Collection(nameof(DbDependentFixtureCollection))]
    public class AddMasterCofoundryUserCommandHandlerTests
    {
        private readonly DbDependentTestApplicationFactory _appFactory;

        public AddMasterCofoundryUserCommandHandlerTests(
            DbDependentTestApplicationFactory appFactory
            )
        {
            _appFactory = appFactory;
        }

        [Fact]
        public async Task WhenAlreadySetup_Throws()
        {
            using var app = _appFactory.Create();
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var command = new AddMasterCofoundryUserCommand()
            {
                Email = "test@example.com",
                Password = "n0tRequired",
                RequirePasswordChange = true
            };

            await contentRepository
                .Awaiting(r => r.ExecuteCommandAsync(command))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMessage("site * set up*");
        }

        [Fact]
        public async Task WhenOtherMasterUsers_Throws()
        {
            using var app = _appFactory.Create(s =>
            {
                s.MockHandler<GetSettingsQuery<InternalSettings>, InternalSettings>(new InternalSettings() { IsSetup = false });
            });
            var contentRepository = app.Services.GetContentRepositoryWithElevatedPermissions();
            var command = new AddMasterCofoundryUserCommand()
            {
                Email = "test@example.com",
                Password = "n0tRequired",
                RequirePasswordChange = true
            };

            await contentRepository
                .Awaiting(r => r.ExecuteCommandAsync(command))
                .Should()
                .ThrowAsync<ValidationException>()
                .WithMessage("* master users already exist *");
        }
    }
}
