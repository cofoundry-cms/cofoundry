using Cofoundry.Domain.Internal;
using Cofoundry.Domain.Tests.Shared;
using Cofoundry.Domain.Tests.Users.Services;
using Cofoundry.Web.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Cofoundry.Web.Tests.Framework.ClientConnection;

public class WebUserSessionServiceTests : UserSessionServiceTests
{
    protected override IUserSessionService CreateService(IUserAreaDefinitionRepository userAreaDefinitionRepository)
    {
        var mockHttpContextAccessor = Substitute.For<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        context.RequestServices = CreateServiceProvider(userAreaDefinitionRepository);
        mockHttpContextAccessor.HttpContext.Returns(context);

        var mockClaimsPrincipalBuilderContextRepository = Substitute.For<IClaimsPrincipalBuilderContextRepository>();
        mockClaimsPrincipalBuilderContextRepository
            .GetAsync(Arg.Any<int>())
            .Returns(x => builderContext(x.Arg<int>()));

        return new WebUserSessionService(
            mockHttpContextAccessor,
            userAreaDefinitionRepository,
            new UserContextCache(),
            new ClaimsPrincipalFactory(),
            mockClaimsPrincipalBuilderContextRepository
            );

        static IClaimsPrincipalBuilderContext builderContext(int i) => new ClaimsPrincipalBuilderContext()
        {
            UserId = i,
            UserAreaCode = i == UserSessionServiceTests.AREA1_USERID ? TestUserArea1.Code : TestUserArea2.Code,
            SecurityStamp = "TEST" + i
        };
    }

    /// <summary>
    /// Creates a minimal service provider to support authentication
    /// </summary>
    private static ServiceProvider CreateServiceProvider(IUserAreaDefinitionRepository userAreaDefinitionRepository)
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
