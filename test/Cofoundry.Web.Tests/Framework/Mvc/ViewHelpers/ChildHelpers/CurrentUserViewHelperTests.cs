using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Web.Tests
{
    public class CurrentUserViewHelperTests
    {
        #region Setup

        private static readonly IUserContext _userAreaContext = new UserContext()
        {
            UserId = 19,
            RoleId = 85,
            UserArea = new CofoundryAdminUserArea(new AdminSettings())
        };

        public CurrentUserViewHelper CreateTestHelper(IUserContext defaultUserContext = null)
        {
            // UserContextService

            var userContextService = new Mock<IUserContextService>();

            if (defaultUserContext == null)
            {
                defaultUserContext = new UserContext();
            }

            userContextService
                .Setup(r => r.GetCurrentContextAsync())
                .ReturnsAsync(() => defaultUserContext);
            userContextService
                .Setup(r => r.GetCurrentContextByUserAreaAsync(It.Is<string>(s => s == _userAreaContext.UserArea.UserAreaCode)))
                .ReturnsAsync(() => _userAreaContext);

            // IQueryExecutor

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

            // IQueryExecutor: defaultUserContext

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

            return new CurrentUserViewHelper(userContextService.Object, queryExecutor.Object);
        }

        #endregion

        [Fact]
        public async Task GetAsync_WhenNotLoggedIn_ReturnsAnonymous()
        {
            // Arrange

            var helper = CreateTestHelper();

            // Act

            var result = await helper.GetAsync();
            var cachedResult = await helper.GetAsync();

            // Assert

            Assert.False(result.IsLoggedIn);
            Assert.True(result.Role.IsAnonymousRole);
            Assert.Null(result.User);
            Assert.Equal(result, cachedResult);
        }

        [Fact]
        public async Task GetAsync_WhenLoggedIn_ReturnsCorrectUser()
        {
            // Arrange

            var userContext = new UserContext()
            {
                UserId = 20,
                RoleId = 55
            };

            var helper = CreateTestHelper(userContext);

            // Act

            var result = await helper.GetAsync();
            var cachedResult = await helper.GetAsync();

            // Assert

            Assert.True(result.IsLoggedIn);
            Assert.False(result.Role.IsAnonymousRole);
            Assert.Equal(userContext.RoleId, result.Role.RoleId);
            Assert.Equal(userContext.UserId, result.User.UserId);
            Assert.Equal(result, cachedResult);
        }

        [Fact]
        public async Task GetAsync_WithAreaCode_ReturnsCorrectUser()
        {
            // Arrange

            var helper = CreateTestHelper();

            // Act

            await helper.GetAsync();
            var result = await helper.GetAsync(_userAreaContext.UserArea.UserAreaCode);
            var cachedResult = await helper.GetAsync(_userAreaContext.UserArea.UserAreaCode);

            // Assert

            Assert.True(result.IsLoggedIn);
            Assert.False(result.Role.IsAnonymousRole);
            Assert.Equal(_userAreaContext.RoleId, result.Role.RoleId);
            Assert.Equal(_userAreaContext.UserId, result.User.UserId);
            Assert.Equal(result, cachedResult);
        }
    }
}
