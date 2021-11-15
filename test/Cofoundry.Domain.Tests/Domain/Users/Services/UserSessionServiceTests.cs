using Cofoundry.Domain.Internal;
using Cofoundry.Domain.Tests.Shared;
using FluentAssertions;
using FluentAssertions.Execution;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Cofoundry.Domain.Tests
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

            userId.Should().BeNull();
        }

        [Fact]
        public async Task GetCurrentUserId_WhenLoggedInToDefault_ReturnsUserId()
        {
            var service = CreateService(CreateUserAreaRepository());

            await service.LogUserInAsync(TestUserArea1.Code, AREA1_USERID, false);
            var userId = service.GetCurrentUserId();

            userId.Should().Be(AREA1_USERID);
        }

        [Fact]
        public virtual async Task GetCurrentUserId_WhenLoggedInToNonDefault_ReturnsNull()
        {
            var service = CreateService(CreateUserAreaRepository());

            await service.LogUserInAsync(TestUserArea2.Code, AREA2_USERID, false);
            var userId = service.GetCurrentUserId();

            userId.Should().BeNull();
        }

        [Fact]
        public async Task GetUserIdByUserAreaCodeAsync_WhenNotLoggedIn_ReturnsNull()
        {
            var service = CreateService(CreateUserAreaRepository());

            var userId1 = await service.GetUserIdByUserAreaCodeAsync(TestUserArea1.Code);
            var userId2 = await service.GetUserIdByUserAreaCodeAsync(TestUserArea2.Code);

            using (new AssertionScope())
            {
                userId1.Should().BeNull();
                userId2.Should().BeNull();
            }
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

            using (new AssertionScope())
            {
                currentUserId.Should().Be(AREA1_USERID);
                userId1.Should().Be(AREA1_USERID);
                userId2.Should().Be(AREA2_USERID);
            }
        }

        [Fact]
        public async Task LogUserOutAsync_WhenLoggedOut_DoesNotThrow()
        {
            var service = CreateService(CreateUserAreaRepository());

            using (new AssertionScope())
            {
                await service
                    .Awaiting(s => s.LogUserOutAsync(TestUserArea1.Code))
                    .Should()
                    .NotThrowAsync();
                await service
                    .Awaiting(s => s.LogUserOutAsync(TestUserArea2.Code))
                    .Should()
                    .NotThrowAsync();
            }
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

            using (new AssertionScope())
            {
                currentUserId.Should().Be(AREA1_USERID);
                userId1.Should().Be(AREA1_USERID);
                userId2.Should().BeNull();
            }
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

            using (new AssertionScope())
            {
                currentUserId.Should().BeNull();
                userId1.Should().BeNull();
                userId2.Should().BeNull();
            }
        }

        [Fact]
        public async Task SetAmbientUserAreaAsync_WhenChangedToNonDefault_CurrentUserIsAmbient()
        {
            const int USER1_ID = 123456;
            const int USER2_ID = 654321;
            var service = CreateService(CreateUserAreaRepository());

            await service.LogUserInAsync(TestUserArea1.Code, USER1_ID, false);
            await service.LogUserInAsync(TestUserArea2.Code, USER2_ID, false);
            await service.SetAmbientUserAreaAsync(TestUserArea2.Code);
            var userId = service.GetCurrentUserId();

            userId.Should().Be(USER2_ID);
        }

        [Fact]
        public async Task SetAmbientUserAreaAsync_WhenReturnedToDefault_CurrentUserIsNull()
        {
            const int USER_ID = 123456;
            var service = CreateService(CreateUserAreaRepository());

            await service.LogUserInAsync(TestUserArea2.Code, USER_ID, false);
            await service.SetAmbientUserAreaAsync(TestUserArea2.Code);
            await service.SetAmbientUserAreaAsync(TestUserArea1.Code);
            var userId = service.GetCurrentUserId();

            userId.Should().BeNull();
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
