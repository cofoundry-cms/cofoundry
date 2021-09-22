using Cofoundry.Domain;
using Cofoundry.Domain.Tests;
using Cofoundry.Domain.Tests.Shared;
using Cofoundry.Web.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Web.Tests.Framework.ClientConnection
{
    public class WebUserSessionServiceTests : UserSessionServiceTests
    {
        [Fact(Skip = "WebUserSessionService works differently because 'current' could mean the ambient authentication scheme rather than the default.")]
        public override Task GetCurrentUserId_WhenLoggedInToNonDefault_ReturnsNull()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// WebUserSessionService works differently to the base test expectations because 'current' could 
        /// mean the ambient authentication scheme (applied via an AuthorizeAttribute on a controller) rather 
        /// than the default.
        /// </summary>
        [Fact]
        public async Task GetCurrentUserId_WhenLoggedInToAmbient_ReturnsUserId()
        {
            const int USER_ID = 123456;
            var service = CreateService(CreateUserAreaRepository());

            await service.LogUserInAsync(TestUserArea2.Code, USER_ID, false);
            var userId = service.GetCurrentUserId();

            Assert.Equal(USER_ID, userId);
        }

        protected override IUserSessionService CreateService(IUserAreaDefinitionRepository userAreaDefinitionRepository)
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.RequestServices = CreateServiceProvider(userAreaDefinitionRepository);
            mockHttpContextAccessor.Setup(m => m.HttpContext).Returns(context);

            return new WebUserSessionService(mockHttpContextAccessor.Object, userAreaDefinitionRepository);
        }

        /// <summary>
        /// Creates a minimal service provider to support authentication
        /// </summary>
        private ServiceProvider CreateServiceProvider(IUserAreaDefinitionRepository userAreaDefinitionRepository)
        {
            var allUserAreas = userAreaDefinitionRepository.GetAll();

            var defaultSchemaCode = userAreaDefinitionRepository.GetDefault().UserAreaCode;
            var defaultScheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(defaultSchemaCode);

            var services = new ServiceCollection();
            var authBuilder = services.AddAuthentication(defaultScheme);

            foreach (var userAreaDefinition in allUserAreas)
            {
                var scheme = CofoundryAuthenticationConstants.FormatAuthenticationScheme(userAreaDefinition.UserAreaCode);
                authBuilder.AddCookie(scheme);
            }

            services.AddLogging(config => config.AddDebug().AddConsole());
            services.AddControllersWithViews();

            return services.BuildServiceProvider();
        }
    }
}
