using Cofoundry.Domain.CQS;
using Cofoundry.Web.Internal;
using NSubstitute;

namespace Cofoundry.Web.Tests;

public class CurrentUserViewHelperTests
{
    private static readonly IUserContext _userAreaContext = new UserContext()
    {
        UserId = 19,
        RoleId = 85,
        UserArea = new CofoundryAdminUserArea(new AdminSettings())
    };

    private static readonly ISignedInUserContext _signedInUserAreaContext = _userAreaContext.ToRequiredSignedInContext();

    [Fact]
    public async Task GetAsync_WhenNotLoggedIn_ReturnsAnonymous()
    {
        var currentUserViewHelper = CreateCurrentUserViewHelper();

        var result = await currentUserViewHelper.GetAsync();
        var cachedResult = await currentUserViewHelper.GetAsync();

        Assert.False(result.IsSignedIn);
        Assert.True(result.Role.IsAnonymousRole);
        Assert.Null(result.Data);
        Assert.Equal(result, cachedResult);
    }

    [Fact]
    public async Task GetAsync_WhenLoggedIn_ReturnsCorrectUser()
    {
        var userContext = new UserContext()
        {
            UserId = 20,
            RoleId = 55,
            UserArea = new CofoundryAdminUserArea(new AdminSettings())
        };

        var currentUserViewHelper = CreateCurrentUserViewHelper(userContext);

        var result = await currentUserViewHelper.GetAsync();
        var cachedResult = await currentUserViewHelper.GetAsync();

        Assert.True(result.IsSignedIn);
        Assert.False(result.Role.IsAnonymousRole);
        Assert.Equal(userContext.RoleId, result.Role.RoleId);
        Assert.Equal(userContext.UserId, result.Data?.UserId);
        Assert.Equal(result, cachedResult);
    }

    [Fact]
    public async Task GetAsync_WithAreaCode_ReturnsCorrectUser()
    {
        var currentUserViewHelper = CreateCurrentUserViewHelper();

        await currentUserViewHelper.GetAsync();
        var result = await currentUserViewHelper.GetAsync(_signedInUserAreaContext.UserArea.UserAreaCode);
        var cachedResult = await currentUserViewHelper.GetAsync(_signedInUserAreaContext.UserArea.UserAreaCode);

        Assert.True(result.IsSignedIn);
        Assert.False(result.Role.IsAnonymousRole);
        Assert.Equal(_userAreaContext.RoleId, result.Role.RoleId);
        Assert.Equal(_userAreaContext.UserId, result.Data?.UserId);
        Assert.Equal(result, cachedResult);
    }

    private static CurrentUserViewHelper CreateCurrentUserViewHelper(IUserContext? defaultUserContext = null)
    {
        defaultUserContext ??= new UserContext();

        var userContextService = MockUserContextService(defaultUserContext);
        var queryExecutor = MockQueryExecutor(defaultUserContext);

        return new CurrentUserViewHelper(userContextService, queryExecutor);
    }

    private static IUserContextService MockUserContextService(IUserContext defaultUserContext)
    {
        var userContextService = Substitute.For<IUserContextService>();

        userContextService
            .GetCurrentContextAsync()
            .Returns(defaultUserContext);

        userContextService
            .GetCurrentContextByUserAreaAsync(Arg.Is(_signedInUserAreaContext.UserArea.UserAreaCode))
            .Returns(_userAreaContext);

        return userContextService;
    }

    private static IQueryExecutor MockQueryExecutor(IUserContext defaultUserContext)
    {
        var queryExecutor = Substitute.For<IQueryExecutor>();
        queryExecutor
            .ExecuteAsync(Arg.Is<GetRoleDetailsByIdQuery>(m => m.RoleId == null), Arg.Is(defaultUserContext))
            .Returns(new RoleDetails()
            {
                RoleId = -1,
                IsAnonymousRole = true,
                Title = "Anonymous",
                UserArea = UserAreaMicroSummary.Uninitialized
            });
        queryExecutor
            .ExecuteAsync(Arg.Is<GetRoleDetailsByIdQuery>(m => m.RoleId == _userAreaContext.RoleId), Arg.Is(_userAreaContext))
            .Returns(new RoleDetails()
            {
                IsAnonymousRole = false,
                RoleId = _signedInUserAreaContext.RoleId,
                Title = "Test Role",
                UserArea = UserAreaMicroSummary.Uninitialized
            });

        queryExecutor
            .ExecuteAsync(Arg.Is<GetUserSummaryByIdQuery>(m => m.UserId == _userAreaContext.UserId), Arg.Is(_userAreaContext))
            .Returns(new UserSummary()
            {
                UserId = _signedInUserAreaContext.UserId
            });

        var defualtSignedInUser = defaultUserContext.ToSignedInContext();
        if (defualtSignedInUser != null)
        {
            queryExecutor
                .ExecuteAsync(Arg.Is<GetRoleDetailsByIdQuery>(m => m.RoleId == defualtSignedInUser.RoleId), Arg.Is(defaultUserContext))
                .Returns(new RoleDetails()
                {
                    IsAnonymousRole = false,
                    RoleId = defualtSignedInUser.RoleId,
                    Title = "Test Role",
                    UserArea = UserAreaMicroSummary.Uninitialized
                });

            queryExecutor
                .ExecuteAsync(Arg.Is<GetUserSummaryByIdQuery>(m => m.UserId == defualtSignedInUser.UserId), Arg.Is(defaultUserContext))
                .Returns(new UserSummary()
                {
                    UserId = defualtSignedInUser.UserId
                });
        }

        return queryExecutor;
    }
}
