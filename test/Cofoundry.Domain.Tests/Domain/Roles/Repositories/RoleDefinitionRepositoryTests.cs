using Cofoundry.Core;
using Cofoundry.Domain.Internal;
using Cofoundry.Domain.Tests.Shared;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cofoundry.Domain.Tests
{
    public class RoleDefinitionRepositoryTests
    {
        [Fact]
        public void Constructor_WhenValid_DoesNotThrows()
        {
            var roleDefinitions = GetBaseRoleDefinitions();

            FluentActions
                .Invoking(() => new RoleDefinitionRepository(roleDefinitions))
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Constructor_WhenDuplicateCode_Throws()
        {
            var roleDefinitions = GetBaseRoleDefinitions();
            var duplicate = roleDefinitions.Last();
            roleDefinitions.Add(new TestRoleDefinition()
            {
                RoleCode = duplicate.RoleCode,
                UserAreaCode = duplicate.UserAreaCode,
                Title = "A unique title"
            });

            FluentActions
                .Invoking(() => new RoleDefinitionRepository(roleDefinitions))
                .Should()
                .Throw<InvalidRoleDefinitionException>()
                .WithMessage($"*{duplicate.RoleCode}*");
        }

        [Fact]
        public void Constructor_WhenDuplicateTitle_Throws()
        {
            var roleDefinitions = GetBaseRoleDefinitions();
            var duplicate = roleDefinitions.Last();
            roleDefinitions.Add(new TestRoleDefinition()
            {
                RoleCode = "UNQ",
                UserAreaCode = duplicate.UserAreaCode,
                Title = duplicate.Title
            });

            FluentActions
                .Invoking(() => new RoleDefinitionRepository(roleDefinitions))
                .Should()
                .Throw<InvalidRoleDefinitionException>()
                .WithMessage($"*{duplicate.Title}*");
        }

        [Theory]
        [InlineData("a silly long code")]
        [InlineData("хороше")]
        [InlineData("      ")]
        [InlineData(null)]
        public void Constructor_WhenInvalidCode_Throws(string code)
        {
            var roleDefinitions = GetBaseRoleDefinitions();
            roleDefinitions.Add(new TestRoleDefinition()
            {
                RoleCode = code,
                Title = "A unique title",
                UserAreaCode = TestUserArea1.Code
            });

            FluentActions
                .Invoking(() => new RoleDefinitionRepository(roleDefinitions))
                .Should()
                .Throw<InvalidRoleDefinitionException>();
        }

        [Fact]
        public void Constructor_WhenNullTitle_Throws()
        {
            var roleDefinitions = GetBaseRoleDefinitions();
            roleDefinitions.Add(new TestRoleDefinition()
            {
                RoleCode = "UNQ",
                Title = null,
                UserAreaCode = TestUserArea1.Code
            });

            FluentActions
                .Invoking(() => new RoleDefinitionRepository(roleDefinitions))
                .Should()
                .Throw<InvalidRoleDefinitionException>()
                .WithMessage($"*does not have a Title*");
        }

        [Fact]
        public void GetAll_WhenEmpty_ReturnsNone()
        {
            var roleDefinitions = Enumerable.Empty<IRoleDefinition>();

            var repo = new RoleDefinitionRepository(roleDefinitions);
            var result = repo.GetAll();

            result.Should().BeEmpty();
        }

        [Fact]
        public void GetAll_WhenNotEmpty_ReturnsAll()
        {
            var roleDefinitions = GetBaseRoleDefinitions();
            var total = roleDefinitions.Count;

            var repo = new RoleDefinitionRepository(roleDefinitions);
            var result = repo.GetAll();

            result.Should().HaveCount(total);
        }

        [Fact]
        public void GetByCode_WhenNotExists_ReturnsNull()
        {
            var roleDefinitions = GetBaseRoleDefinitions();

            var repo = new RoleDefinitionRepository(roleDefinitions);
            var result = repo.GetByCode(TestUserArea1.Code, "UNQ");

            result.Should().BeNull();
        }

        [Theory]
        [InlineData(TestUserArea1.Code, "001")]
        [InlineData(TestUserArea2.Code, "003")]
        public void GetByCode_WhenExists_Returns(string userAreaCode, string roleCode)
        {
            var roleDefinitions = GetBaseRoleDefinitions();

            var repo = new RoleDefinitionRepository(roleDefinitions);
            var result = repo.GetByCode(userAreaCode, roleCode);

            using (new AssertionScope())
            {
                result.UserAreaCode.Should().Be(userAreaCode);
                result.RoleCode.Should().Be(roleCode);
            }
        }

        [Fact]
        public void GetRequiredByCode_WhenNotExists_Throws()
        {
            var roleDefinitions = GetBaseRoleDefinitions();
            var repo = new RoleDefinitionRepository(roleDefinitions);

            repo.Invoking(r => r.GetRequiredByCode(TestUserArea1.Code, "UNIQUE"))
                .Should()
                .Throw<EntityNotFoundException<IRoleDefinition>>();
        }

        [Theory]
        [InlineData(TestUserArea1.Code, "001")]
        [InlineData(TestUserArea2.Code, "003")]
        public void GetRequiredByCode_WhenExists_Returns(string userAreaCode, string roleCode)
        {
            var roleDefinitions = GetBaseRoleDefinitions();

            var repo = new RoleDefinitionRepository(roleDefinitions);
            var result = repo.GetByCode(userAreaCode, roleCode);

            using (new AssertionScope())
            {
                result.UserAreaCode.Should().Be(userAreaCode);
                result.RoleCode.Should().Be(roleCode);
            }
        }

        private class TestRoleDefinition : IRoleDefinition
        {
            public string Title { get; set; }

            public string RoleCode { get; set; }

            public string UserAreaCode { get; set; }

            public void ConfigurePermissions(IPermissionSetBuilder builder)
            {
                // all permissions
            }
        }

        private List<IRoleDefinition> GetBaseRoleDefinitions()
        {
            return new List<IRoleDefinition>()
            {
                new TestRoleDefinition()
                {
                    RoleCode = "001",
                    Title = "Role 1",
                    UserAreaCode = TestUserArea1.Code
                },
                new TestRoleDefinition()
                {
                    RoleCode = "002",
                    Title = "Role 2",
                    UserAreaCode = TestUserArea1.Code
                },
                new TestRoleDefinition()
                {
                    RoleCode = "001",
                    Title = "Role 1",
                    UserAreaCode = TestUserArea2.Code
                },
                new TestRoleDefinition()
                {
                    RoleCode = "003",
                    Title = "Role 3",
                    UserAreaCode = TestUserArea2.Code
                },
            };
        }
    }
}
