using Cofoundry.Domain.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Cofoundry.Domain.Tests
{
    public class EntityDefinitionRepositoryTests
    {
        #region helpers

        private class TestEntityDefinition : IEntityDefinition
        {
            public string EntityDefinitionCode { get; set; }

            public string Name { get; set; }
        }

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

        private List<IEntityDefinition> GetBaseEntityDefinitions()
        {
            return new List<IEntityDefinition>()
            {
                new TestEntityDefinition()
                {
                    EntityDefinitionCode = "TST001",
                    Name = "Test 1"
                },
                new TestEntityDefinition()
                {
                    EntityDefinitionCode = "TST002",
                    Name = "Test 2"
                },
                new TestEntityDefinition()
                {
                    EntityDefinitionCode = "TST003",
                    Name = "Test 3"
                },
            };
        }

        private ICustomEntityDefinitionRepository GetCustomEntityRepository()
        {
            var mock = new Mock<ICustomEntityDefinitionRepository>();
            var customEntityDefinitions = GetBaseCustomEntityDefinitions();
            mock.Setup(r => r.GetAll()).Returns(() => customEntityDefinitions);

            return mock.Object;
        }

        #endregion

        #region EntityDefinitionRepository constructor

        [Fact]
        public void Constructor_WhenDuplicateCode_Throws()
        {
            var customEntityRepository = GetCustomEntityRepository();
            var entityDefinitions = GetBaseEntityDefinitions();
            entityDefinitions.Add(new TestEntityDefinition()
            {
                EntityDefinitionCode = entityDefinitions.First().EntityDefinitionCode,
                Name = "A unique name"
            });

            Assert.Throws<InvalidEntityDefinitionException>(() => new EntityDefinitionRepository(entityDefinitions, customEntityRepository));
        }

        [Fact]
        public void Constructor_WhenDuplicateName_Throws()
        {
            var customEntityRepository = GetCustomEntityRepository();
            var entityDefinitions = GetBaseEntityDefinitions();
            entityDefinitions.Add(new TestEntityDefinition()
            {
                EntityDefinitionCode = "UNIQUE",
                Name = entityDefinitions.First().Name
            });

            Assert.Throws<InvalidEntityDefinitionException>(() => new EntityDefinitionRepository(entityDefinitions, customEntityRepository));
        }

        [Theory]
        [InlineData("a silly long code")]
        [InlineData("хороше")]
        [InlineData("      ")]
        [InlineData(null)]
        public void Constructor_WhenInvalidCode_Throws(string code)
        {
            var customEntityRepository = GetCustomEntityRepository();
            var entityDefinitions = GetBaseEntityDefinitions();
            entityDefinitions.Add(new TestEntityDefinition()
            {
                EntityDefinitionCode = code,
                Name = "A unique name"
            });

            Assert.Throws<InvalidEntityDefinitionException>(() => new EntityDefinitionRepository(entityDefinitions, customEntityRepository));
        }

        [Fact]
        public void Constructor_WhenNullName_Throws()
        {
            var customEntityRepository = GetCustomEntityRepository();
            var entityDefinitions = GetBaseEntityDefinitions();
            entityDefinitions.Add(new TestEntityDefinition()
            {
                EntityDefinitionCode = "UNIQUE",
                Name = null
            });

            Assert.Throws<InvalidEntityDefinitionException>(() => new EntityDefinitionRepository(entityDefinitions, customEntityRepository));
        }

        #endregion

        #region GetAll

        [Fact]
        public void GetAll_WhenEmpty_ReturnsNone()
        {
            var mock = new Mock<ICustomEntityDefinitionRepository>();
            mock.Setup(r => r.GetAll()).Returns(() => Enumerable.Empty<ICustomEntityDefinition>());

            var customEntityRepository = mock.Object;
            var entityDefinitions = Enumerable.Empty<IEntityDefinition>();

            var repo = new EntityDefinitionRepository(entityDefinitions, customEntityRepository);
            var result = repo.GetAll();

            Assert.Empty(result);
        }

        [Fact]
        public void GetAll_WhenNotEmpty_ReturnsAll()
        {
            var customEntityRepository = GetCustomEntityRepository();
            var entityDefinitions = GetBaseEntityDefinitions();
            var total = customEntityRepository.GetAll().Count() + entityDefinitions.Count;

            var repo = new EntityDefinitionRepository(entityDefinitions, customEntityRepository);
            var result = repo.GetAll();

            Assert.Equal(total, result.Count());
        }

        #endregion

        #region GetByCode

        [Fact]
        public void GetByCode_WhenNotExists_ReturnsNull()
        {
            var customEntityRepository = GetCustomEntityRepository();
            var entityDefinitions = GetBaseEntityDefinitions();

            var repo = new EntityDefinitionRepository(entityDefinitions, customEntityRepository);
            var result = repo.GetByCode("UNIQUE");

            Assert.Null(result);
        }

        [Theory]
        [InlineData("TST003")]
        [InlineData("CUS002")]
        public void GetByCode_WhenExists_Returns(string definitionCode)
        {
            var customEntityRepository = GetCustomEntityRepository();
            var entityDefinitions = GetBaseEntityDefinitions();

            var repo = new EntityDefinitionRepository(entityDefinitions, customEntityRepository);
            var result = repo.GetByCode(definitionCode);

            Assert.Equal(definitionCode, result.EntityDefinitionCode);
        }

        #endregion
    }
}
