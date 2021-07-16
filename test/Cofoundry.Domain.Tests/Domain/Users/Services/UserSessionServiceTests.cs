using Cofoundry.Domain.Internal;
using Cofoundry.Domain.Tests.Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests.Domain
{
    public abstract class UserSessionServiceTests
    {
        const int AREA1_USERID = 2984;
        const int AREA2_USERID = 838991;

        [Fact]
        public void GetCurrentUserId_WhenNotLoggedIn_ReturnsNull()
        {
            var service = CreateService(CreateUserAreaRepository());

            var userId = service.GetCurrentUserId();

            Assert.Null(userId);
        }

        [Fact]
        public async Task GetCurrentUserId_WhenLoggedInToDefault_ReturnsUserId()
        {
            var service = CreateService(CreateUserAreaRepository());

            await service.LogUserInAsync(TestUserArea1.Code, AREA1_USERID, false);
            var userId = service.GetCurrentUserId();

            Assert.Equal(AREA1_USERID, userId);
        }

        [Fact]
        public virtual async Task GetCurrentUserId_WhenLoggedInToNonDefault_ReturnsNull()
        {
            var service = CreateService(CreateUserAreaRepository());

            await service.LogUserInAsync(TestUserArea2.Code, AREA2_USERID, false);
            var userId = service.GetCurrentUserId();

            Assert.Null(userId);
        }

        [Fact]
        public async Task GetUserIdByUserAreaCodeAsync_WhenNotLoggedIn_ReturnsNull()
        {
            var service = CreateService(CreateUserAreaRepository());

            var userId1 = await service.GetUserIdByUserAreaCodeAsync(TestUserArea1.Code);
            var userId2 = await service.GetUserIdByUserAreaCodeAsync(TestUserArea2.Code);

            Assert.Null(userId1);
            Assert.Null(userId2);
        }

        [Fact]
        public async Task GetUserIdByUserAreaCodeAsync_WhenLoggedInToMultiple_ReturnsUserId()
        {
            var service = CreateService(CreateUserAreaRepository());

            await service.LogUserInAsync(TestUserArea1.Code, AREA1_USERID, false);
            await service.LogUserInAsync(TestUserArea2.Code, AREA2_USERID, false);

            var currentUserId = service.GetCurrentUserId();
            var userId1 = await service.GetUserIdByUserAreaCodeAsync(TestUserArea1.Code);
            var userId2 = await service.GetUserIdByUserAreaCodeAsync(TestUserArea2.Code);

            Assert.Equal(AREA1_USERID, currentUserId);
            Assert.Equal(AREA1_USERID, userId1);
            Assert.Equal(AREA2_USERID, userId2);
        }

        [Fact]
        public async Task LogUserOutAsync_WhenLoggedOut_DoesNotThrow()
        {
            var service = CreateService(CreateUserAreaRepository());

            await service.LogUserOutAsync(TestUserArea1.Code);
            await service.LogUserOutAsync(TestUserArea2.Code);
        }

        [Fact]
        public async Task LogUserOutAsync_WhenLoggedInToMultiple_LogsOutSpecifiedAreaOnly()
        {
            var service = CreateService(CreateUserAreaRepository());

            await service.LogUserInAsync(TestUserArea1.Code, AREA1_USERID, false);
            await service.LogUserInAsync(TestUserArea2.Code, AREA2_USERID, false);
            await service.LogUserOutAsync(TestUserArea2.Code);

            var currentUserId = service.GetCurrentUserId();
            var userId1 = await service.GetUserIdByUserAreaCodeAsync(TestUserArea1.Code);
            var userId2 = await service.GetUserIdByUserAreaCodeAsync(TestUserArea2.Code);

            Assert.Equal(AREA1_USERID, currentUserId);
            Assert.Equal(AREA1_USERID, userId1);
            Assert.Null(userId2);
        }

        [Fact]
        public async Task LogUserOutOfAllUserAreasAsync_WhenLoggedOut_DoesNotThrow()
        {
            var service = CreateService(CreateUserAreaRepository());

            await service.LogUserOutOfAllUserAreasAsync();
        }

        [Fact]
        public async Task LogUserOutAsync_WhenLoggedInToMultiple_LogsOutAll()
        {
            var service = CreateService(CreateUserAreaRepository());

            await service.LogUserInAsync(TestUserArea1.Code, AREA1_USERID, false);
            await service.LogUserInAsync(TestUserArea2.Code, AREA2_USERID, false);
            await service.LogUserOutOfAllUserAreasAsync();

            var currentUserId = service.GetCurrentUserId();
            var userId1 = await service.GetUserIdByUserAreaCodeAsync(TestUserArea1.Code);
            var userId2 = await service.GetUserIdByUserAreaCodeAsync(TestUserArea2.Code);

            Assert.Null(currentUserId);
            Assert.Null(userId1);
            Assert.Null(userId2);
        }


        protected abstract IUserSessionService CreateService(IUserAreaDefinitionRepository repository);

        protected IUserAreaDefinitionRepository CreateUserAreaRepository()
        {
            var areas = new IUserAreaDefinition[]
            {
                new TestUserArea1(), new TestUserArea2()
            };
            
            return new UserAreaDefinitionRepository(areas);
        }
    }
}
