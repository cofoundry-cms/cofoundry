using Cofoundry.Domain.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Cofoundry.Domain.Tests
{
    public class CustomEntityDefinitionRepositoryTests
    {
        #region helpers

        private class TestCustomEntityDefinition : ICustomEntityDefinition
        {
            public string Name { get; set; }

            public string CustomEntityDefinitionCode { get; set; }

            public string NamePlural => throw new NotImplementedException();

            public string Description => throw new NotImplementedException();

            public bool ForceUrlSlugUniqueness => throw new NotImplementedException();

            public bool HasLocale => throw new NotImplementedException();

            public bool AutoGenerateUrlSlug => throw new NotImplementedException();

            public bool AutoPublish => throw new NotImplementedException();
        }

        private List<ICustomEntityDefinition> GetBaseCustomEntityDefinitions()
        {
            return new List<ICustomEntityDefinition>()
            {
                new TestCustomEntityDefinition()
                {
                    CustomEntityDefinitionCode = "CUS001",
                    Name = "Test CE 1"
                },
                new TestCustomEntityDefinition()
                {
                    CustomEntityDefinitionCode = "CUS002",
                    Name = "Test CE 2"
                },
                new TestCustomEntityDefinition()
                {
                    CustomEntityDefinitionCode = "CUS003",
                    Name = "Test CE 3"
                },
            };
        }

        #endregion

        #region CustomEntityDefinitionRepository constructor

        [Fact]
        public void Constructor_WhenDuplicateCode_Throws()
        {
            var entityDefinitions = GetBaseCustomEntityDefinitions();
            entityDefinitions.Add(new TestCustomEntityDefinition()
            {
                CustomEntityDefinitionCode = entityDefinitions.First().CustomEntityDefinitionCode,
                Name = "A unique name"
            });

            Assert.Throws<InvalidCustomEntityDefinitionException>(() => new CustomEntityDefinitionRepository(entityDefinitions));
        }

        [Fact]
        public void Constructor_WhenDuplicateName_Throws()
        {
            var entityDefinitions = GetBaseCustomEntityDefinitions();
            entityDefinitions.Add(new TestCustomEntityDefinition()
            {
                CustomEntityDefinitionCode = "UNIQUE",
                Name = entityDefinitions.First().Name
            });

            Assert.Throws<InvalidCustomEntityDefinitionException>(() => new CustomEntityDefinitionRepository(entityDefinitions));
        }

        [Theory]
        [InlineData("a silly long code")]
        [InlineData("хороше")]
        [InlineData("      ")]
        [InlineData(null)]
        public void Constructor_WhenInvalidCode_Throws(string code)
        {
            var entityDefinitions = GetBaseCustomEntityDefinitions();
            entityDefinitions.Add(new TestCustomEntityDefinition()
            {
                CustomEntityDefinitionCode = code,
                Name = "A unique name"
            });

            Assert.Throws<InvalidCustomEntityDefinitionException>(() => new CustomEntityDefinitionRepository(entityDefinitions));
        }

        [Fact]
        public void Constructor_WhenNullName_Throws()
        {
            var entityDefinitions = GetBaseCustomEntityDefinitions();
            entityDefinitions.Add(new TestCustomEntityDefinition()
            {
                CustomEntityDefinitionCode = "UNIQUE",
                Name = null
            });

            Assert.Throws<InvalidCustomEntityDefinitionException>(() => new CustomEntityDefinitionRepository(entityDefinitions));
        }

        #endregion

        #region GetAll

        [Fact]
        public void GetAll_WhenEmpty_ReturnsNone()
        {
            var entityDefinitions = Enumerable.Empty<ICustomEntityDefinition>();

            var repo = new CustomEntityDefinitionRepository(entityDefinitions);
            var result = repo.GetAll();

            Assert.Empty(result);
        }

        [Fact]
        public void GetAll_WhenNotEmpty_ReturnsAll()
        {
            var entityDefinitions = GetBaseCustomEntityDefinitions();
            var total = entityDefinitions.Count;

            var repo = new CustomEntityDefinitionRepository(entityDefinitions);
            var result = repo.GetAll();

            Assert.Equal(total, result.Count());
        }

        #endregion

        #region GetByCode

        [Fact]
        public void GetByCode_WhenNotExists_ReturnsNull()
        {
            var entityDefinitions = GetBaseCustomEntityDefinitions();

            var repo = new CustomEntityDefinitionRepository(entityDefinitions);
            var result = repo.GetByCode("UNIQUE");

            Assert.Null(result);
        }

        [Theory]
        [InlineData("CUS001")]
        [InlineData("CUS003")]
        public void GetByCode_WhenExists_Returns(string definitionCode)
        {
            var entityDefinitions = GetBaseCustomEntityDefinitions();

            var repo = new CustomEntityDefinitionRepository(entityDefinitions);
            var result = repo.GetByCode(definitionCode);

            Assert.Equal(definitionCode, result.CustomEntityDefinitionCode);
        }

        #endregion
    }
}
