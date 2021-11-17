using Cofoundry.Domain;
using Cofoundry.Domain.Internal;
using Cofoundry.Domain.Tests;
using Cofoundry.Web.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace Cofoundry.Web.Tests.Framework.ClientConnection
{
    public class WebUserSessionServiceTests : UserSessionServiceTests
    {
        protected override IUserSessionService CreateService(IUserAreaDefinitionRepository userAreaDefinitionRepository)
        {
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            context.RequestServices = CreateServiceProvider(userAreaDefinitionRepository);
            mockHttpContextAccessor.Setup(m => m.HttpContext).Returns(context);

            return new WebUserSessionService(mockHttpContextAccessor.Object, userAreaDefinitionRepository, new UserContextCache());
        }

        /// <summary>
        /// Creates a minimal service provider to support authentication
        /// </summary>
        private ServiceProvider CreateServiceProvider(IUserAreaDefinitionRepository userAreaDefinitionRepository)
        {
            var allUserAreas = userAreaDefinitionRepository.GetAll();

            var defaultSchemaCode = userAreaDefinitionRepository.GetDefault().UserAreaCode;
            var defaultScheme = AuthenticationSchemeNames.UserArea(defaultSchemaCode);

            var services = new ServiceCollection();
            var authBuilder = services.AddAuthentication(defaultScheme);

            foreach (var userAreaDefinition in allUserAreas)
            {
                var scheme = AuthenticationSchemeNames.UserArea(userAreaDefinition.UserAreaCode);
                authBuilder.AddCookie(scheme);
            }

            services.AddLogging(config => config.AddDebug().AddConsole());
            services.AddControllersWithViews();

            return services.BuildServiceProvider();
        }
    }
}
