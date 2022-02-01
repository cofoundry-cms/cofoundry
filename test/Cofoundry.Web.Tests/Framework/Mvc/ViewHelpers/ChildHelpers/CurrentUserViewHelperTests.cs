using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Web.Tests
{
    public class CurrentUserViewHelperTests
    {
        private static readonly IUserContext _userAreaContext = new UserContext()
        {
            UserId = 19,
            RoleId = 85,
            UserArea = new CofoundryAdminUserArea(new AdminSettings())
        };

        [Fact]
        public async Task GetAsync_WhenNotLoggedIn_ReturnsAnonymous()
        {
            var currentUserViewHelper = CreateCurrentUserViewHelper();

            var result = await currentUserViewHelper.GetAsync();
            var cachedResult = await currentUserViewHelper.GetAsync();

            Assert.False(result.IsLoggedIn);
            Assert.True(result.Role.IsAnonymousRole);
            Assert.Null(result.User);
            Assert.Equal(result, cachedResult);
        }

        [Fact]
        public async Task GetAsync_WhenLoggedIn_ReturnsCorrectUser()
        {
            var userContext = new UserContext()
            {
                UserId = 20,
                RoleId = 55
            };

            var currentUserViewHelper = CreateCurrentUserViewHelper(userContext);

            var result = await currentUserViewHelper.GetAsync();
            var cachedResult = await currentUserViewHelper.GetAsync();

            Assert.True(result.IsLoggedIn);
            Assert.False(result.Role.IsAnonymousRole);
            Assert.Equal(userContext.RoleId, result.Role.RoleId);
            Assert.Equal(userContext.UserId, result.User.UserId);
            Assert.Equal(result, cachedResult);
        }

        [Fact]
        public async Task GetAsync_WithAreaCode_ReturnsCorrectUser()
        {
            var currentUserViewHelper = CreateCurrentUserViewHelper();

            await currentUserViewHelper.GetAsync();
            var result = await currentUserViewHelper.GetAsync(_userAreaContext.UserArea.UserAreaCode);
            var cachedResult = await currentUserViewHelper.GetAsync(_userAreaContext.UserArea.UserAreaCode);

            Assert.True(result.IsLoggedIn);
            Assert.False(result.Role.IsAnonymousRole);
            Assert.Equal(_userAreaContext.RoleId, result.Role.RoleId);
            Assert.Equal(_userAreaContext.UserId, result.User.UserId);
            Assert.Equal(result, cachedResult);
        }

        private CurrentUserViewHelper CreateCurrentUserViewHelper(IUserContext defaultUserContext = null)
        {
            if (defaultUserContext == null)
            {
                defaultUserContext = new UserContext();
            }

            var userContextService = MockUserContextService(defaultUserContext);
            var queryExecutor = MockQueryExecutor(defaultUserContext);

            return new CurrentUserViewHelper(userContextService, queryExecutor);
        }

        private static IUserContextService MockUserContextService(IUserContext defaultUserContext)
        {
            var userContextService = new Mock<IUserContextService>();

            userContextService
                .Setup(r => r.GetCurrentContextAsync())
                .ReturnsAsync(() => defaultUserContext);

            userContextService
                .Setup(r => r.GetCurrentContextByUserAreaAsync(It.Is<string>(s => s == _userAreaContext.UserArea.UserAreaCode)))
                .ReturnsAsync(() => _userAreaContext);

            return userContextService.Object;
        }

        private static IQueryExecutor MockQueryExecutor(IUserContext defaultUserContext)
        {
            var queryExecutor = new Mock<IQueryExecutor>();
            queryExecutor
                .Setup(r => r.ExecuteAsync(It.Is<GetRoleDetailsByIdQuery>(m => m.RoleId == null), It.Is<IUserContext>(m => m == defaultUserContext)))
                .ReturnsAsync(() => new RoleDetails() { IsAnonymousRole = true });
            queryExecutor
                .Setup(r => r.ExecuteAsync(It.Is<GetRoleDetailsByIdQuery>(m => m.RoleId == _userAreaContext.RoleId), It.Is<IUserContext>(m => m == _userAreaContext)))
                .ReturnsAsync(() => new RoleDetails()
                {
                    IsAnonymousRole = false,
                    RoleId = _userAreaContext.RoleId.Value
                });

            queryExecutor
                .Setup(r => r.ExecuteAsync(It.Is<GetUserMicroSummaryByIdQuery>(m => m.UserId == _userAreaContext.UserId), It.Is<IUserContext>(m => m == _userAreaContext)))
                .ReturnsAsync(() => new UserMicroSummary()
                {
                    UserId = _userAreaContext.UserId.Value
                });

            if (defaultUserContext.UserId.HasValue)
            {
                queryExecutor
                    .Setup(r => r.ExecuteAsync(It.Is<GetRoleDetailsByIdQuery>(m => m.RoleId == defaultUserContext.RoleId.Value), It.Is<IUserContext>(m => m == defaultUserContext)))
                    .ReturnsAsync(() => new RoleDetails()
                    {
                        IsAnonymousRole = false,
                        RoleId = defaultUserContext.RoleId.Value
                    });

                queryExecutor
                    .Setup(r => r.ExecuteAsync(It.Is<GetUserMicroSummaryByIdQuery>(m => m.UserId == defaultUserContext.UserId.Value), It.Is<IUserContext>(m => m == defaultUserContext)))
                    .ReturnsAsync(new UserMicroSummary()
                    {
                        UserId = defaultUserContext.UserId.Value
                    });
            }

            return queryExecutor.Object;
        }
    }
}
