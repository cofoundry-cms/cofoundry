using Cofoundry.Core;
using Cofoundry.Domain.Internal;
using FluentAssertions;
using FluentAssertions.Execution;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Cofoundry.Domain.Tests
{
    public class AuthorizedTaskTypeDefinitionRepositoryTests
    {
        [Fact]
        public void Constructor_WhenValid_DoesNotThrows()
        {
            var definitions = GetDefinitions();

            FluentActions
                .Invoking(() => new AuthorizedTaskTypeDefinitionRepository(definitions))
                .Should()
                .NotThrow();
        }

        [Fact]
        public void Constructor_WhenDuplicateCode_Throws()
        {
            var definitions = GetDefinitions();
            var duplicate = definitions.Last();
            definitions.Add(new TestAuthorizedTaskTypeDefinition()
            {
                AuthorizedTaskTypeCode = duplicate.AuthorizedTaskTypeCode,
                Name = "A unique name"
            });

            FluentActions
                .Invoking(() => new AuthorizedTaskTypeDefinitionRepository(definitions))
                .Should()
                .Throw<InvalidAuthorizedTaskTypeDefinitionException>()
                .WithMessage($"*{duplicate.AuthorizedTaskTypeCode}*");
        }

        [Fact]
        public void Constructor_WhenDuplicateName_Throws()
        {
            var definitions = GetDefinitions();
            var duplicate = definitions.Last();
            definitions.Add(new TestAuthorizedTaskTypeDefinition()
            {
                AuthorizedTaskTypeCode = "UNIQUE",
                Name = duplicate.Name
            });

            FluentActions
                .Invoking(() => new AuthorizedTaskTypeDefinitionRepository(definitions))
                .Should()
                .Throw<InvalidAuthorizedTaskTypeDefinitionException>()
                .WithMessage($"*{duplicate.Name}*");
        }

        [Theory]
        [InlineData("a silly long code")]
        [InlineData("хороше")]
        [InlineData("      ")]
        [InlineData(null)]
        public void Constructor_WhenInvalidCode_Throws(string code)
        {
            var definitions = GetDefinitions();
            definitions.Add(new TestAuthorizedTaskTypeDefinition()
            {
                AuthorizedTaskTypeCode = code,
                Name = "A unique name"
            });

            FluentActions
                .Invoking(() => new AuthorizedTaskTypeDefinitionRepository(definitions))
                .Should()
                .Throw<InvalidAuthorizedTaskTypeDefinitionException>();
        }

        [Fact]
        public void Constructor_WhenNullTitle_Throws()
        {
            var definitions = GetDefinitions();
            definitions.Add(new TestAuthorizedTaskTypeDefinition()
            {
                AuthorizedTaskTypeCode = "UNIQUE",
                Name = null
            });

            FluentActions
                .Invoking(() => new AuthorizedTaskTypeDefinitionRepository(definitions))
                .Should()
                .Throw<InvalidAuthorizedTaskTypeDefinitionException>()
                .WithMessage($"*does not have a name*");
        }

        [Fact]
        public void GetAll_WhenEmpty_ReturnsNone()
        {
            var definitions = Enumerable.Empty<IAuthorizedTaskTypeDefinition>();

            var repo = new AuthorizedTaskTypeDefinitionRepository(definitions);
            var result = repo.GetAll();

            result.Should().BeEmpty();
        }

        [Fact]
        public void GetAll_WhenNotEmpty_ReturnsAll()
        {
            var definitions = GetDefinitions();
            var total = definitions.Count;

            var repo = new AuthorizedTaskTypeDefinitionRepository(definitions);
            var result = repo.GetAll();

            result.Should().HaveCount(total);
        }

        [Fact]
        public void GetByCode_WhenNotExists_ReturnsNull()
        {
            var definitions = GetDefinitions();

            var repo = new AuthorizedTaskTypeDefinitionRepository(definitions);
            var result = repo.GetByCode("UNIQUE");

            result.Should().BeNull();
        }

        [Theory]
        [InlineData("TST001")]
        [InlineData("TST003")]
        public void GetByCode_WhenExists_Returns(string AuthorizedTaskTypeCode)
        {
            var definitions = GetDefinitions();

            var repo = new AuthorizedTaskTypeDefinitionRepository(definitions);
            var result = repo.GetByCode(AuthorizedTaskTypeCode);

            using (new AssertionScope())
            {
                result.AuthorizedTaskTypeCode.Should().Be(AuthorizedTaskTypeCode);
            }
        }

        [Fact]
        public void GetRequiredByCode_WhenNotExists_Throws()
        {
            var definitions = GetDefinitions();
            var repo = new AuthorizedTaskTypeDefinitionRepository(definitions);

            repo.Invoking(r => r.GetRequiredByCode("UNIQUE"))
                .Should()
                .Throw<EntityNotFoundException<IAuthorizedTaskTypeDefinition>>();
        }

        [Theory]
        [InlineData("TST001")]
        [InlineData("TST003")]
        public void GetRequiredByCode_WhenExists_Returns(string AuthorizedTaskTypeCode)
        {
            var definitions = GetDefinitions();

            var repo = new AuthorizedTaskTypeDefinitionRepository(definitions);
            var result = repo.GetByCode(AuthorizedTaskTypeCode);

            using (new AssertionScope())
            {
                result.AuthorizedTaskTypeCode.Should().Be(AuthorizedTaskTypeCode);
            }
        }

        private class TestAuthorizedTaskTypeDefinition : IAuthorizedTaskTypeDefinition
        {
            public string AuthorizedTaskTypeCode { get; set; }

            public string Name { get; set; }
        }

        private List<IAuthorizedTaskTypeDefinition> GetDefinitions()
        {
            return new List<IAuthorizedTaskTypeDefinition>()
            {
                new TestAuthorizedTaskTypeDefinition()
                {
                    AuthorizedTaskTypeCode = "TST001",
                    Name = "Task 1"
                },
                new TestAuthorizedTaskTypeDefinition()
                {
                    AuthorizedTaskTypeCode = "TST002",
                    Name = "Task 2"
                },
                new TestAuthorizedTaskTypeDefinition()
                {
                    AuthorizedTaskTypeCode = "TST003",
                    Name = "Task 3"
                },
            };
        }
    }
}
