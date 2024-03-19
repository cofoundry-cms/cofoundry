﻿using Cofoundry.Domain.Internal;
using Cofoundry.Domain.Tests.Shared;
using Cofoundry.Domain.Tests.Users.Services;
using Cofoundry.Web.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;

namespace Cofoundry.Web.Tests.Framework.ClientConnection;

public class WebUserSessionServiceTests : UserSessionServiceTests
{
    protected override IUserSessionService CreateService(IUserAreaDefinitionRepository userAreaDefinitionRepository)
    {
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        context.RequestServices = CreateServiceProvider(userAreaDefinitionRepository);
        mockHttpContextAccessor.Setup(m => m.HttpContext).Returns(context);

        var mockClaimsPrincipalBuilderContextRepository = new Mock<IClaimsPrincipalBuilderContextRepository>();
        mockClaimsPrincipalBuilderContextRepository.Setup(m => m.GetAsync(It.IsAny<int>())).ReturnsAsync((Func<int, IClaimsPrincipalBuilderContext>)builderContext);

        return new WebUserSessionService(
            mockHttpContextAccessor.Object,
            userAreaDefinitionRepository,
            new UserContextCache(),
            new ClaimsPrincipalFactory(),
            mockClaimsPrincipalBuilderContextRepository.Object
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
