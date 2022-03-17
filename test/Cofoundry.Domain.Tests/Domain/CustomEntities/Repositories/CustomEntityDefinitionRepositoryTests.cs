using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Tests;

public class CustomEntityDefinitionRepositoryTests
{
    [Fact]
    public void Constructor_WhenDuplicateCode_Throws()
    {
        var entityDefinitions = GetBaseCustomEntityDefinitions();
        var duplicateCode = entityDefinitions.Last().CustomEntityDefinitionCode;
        entityDefinitions.Add(new TestCustomEntityDefinition()
        {
            CustomEntityDefinitionCode = duplicateCode,
            Name = "A unique name"
        });

        FluentActions
            .Invoking(() => new CustomEntityDefinitionRepository(entityDefinitions))
            .Should()
            .Throw<InvalidCustomEntityDefinitionException>()
            .WithMessage($"*{duplicateCode}*");
    }

    [Fact]
    public void Constructor_WhenDuplicateName_Throws()
    {
        var entityDefinitions = GetBaseCustomEntityDefinitions();
        var duplicateName = entityDefinitions.Last().Name;
        entityDefinitions.Add(new TestCustomEntityDefinition()
        {
            CustomEntityDefinitionCode = "UNIQUE",
            Name = duplicateName
        });

        FluentActions
            .Invoking(() => new CustomEntityDefinitionRepository(entityDefinitions))
            .Should()
            .Throw<InvalidCustomEntityDefinitionException>()
            .WithMessage($"*{duplicateName}*");
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

        FluentActions
            .Invoking(() => new CustomEntityDefinitionRepository(entityDefinitions))
            .Should()
            .Throw<InvalidCustomEntityDefinitionException>();
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

        FluentActions
            .Invoking(() => new CustomEntityDefinitionRepository(entityDefinitions))
            .Should()
            .Throw<InvalidCustomEntityDefinitionException>()
            .WithMessage($"*does not have a name*");
    }

    [Fact]
    public void GetAll_WhenEmpty_ReturnsNone()
    {
        var entityDefinitions = Enumerable.Empty<ICustomEntityDefinition>();

        var repo = new CustomEntityDefinitionRepository(entityDefinitions);
        var result = repo.GetAll();

        result.Should().BeEmpty();
    }

    [Fact]
    public void GetAll_WhenNotEmpty_ReturnsAll()
    {
        var entityDefinitions = GetBaseCustomEntityDefinitions();
        var total = entityDefinitions.Count;

        var repo = new CustomEntityDefinitionRepository(entityDefinitions);
        var result = repo.GetAll();

        result.Should().HaveCount(total);
    }

    [Fact]
    public void GetByCode_WhenNotExists_ReturnsNull()
    {
        var entityDefinitions = GetBaseCustomEntityDefinitions();

        var repo = new CustomEntityDefinitionRepository(entityDefinitions);
        var result = repo.GetByCode("UNIQUE");

        result.Should().BeNull();
    }

    [Theory]
    [InlineData("CUS001")]
    [InlineData("CUS003")]
    public void GetByCode_WhenExists_Returns(string definitionCode)
    {
        var entityDefinitions = GetBaseCustomEntityDefinitions();

        var repo = new CustomEntityDefinitionRepository(entityDefinitions);
        var result = repo.GetByCode(definitionCode);

        result.CustomEntityDefinitionCode.Should().Be(definitionCode);
    }

    [Fact]
    public void GetRequiredByCode_WhenNotExists_Throws()
    {
        var entityDefinitions = GetBaseCustomEntityDefinitions();
        var repo = new CustomEntityDefinitionRepository(entityDefinitions);

        repo.Invoking(r => r.GetRequiredByCode("UNIQUE"))
            .Should()
            .Throw<EntityNotFoundException<ICustomEntityDefinition>>();
    }

    [Theory]
    [InlineData("CUS001")]
    [InlineData("CUS003")]
    public void GetRequiredByCode_WhenExists_Returns(string definitionCode)
    {
        var entityDefinitions = GetBaseCustomEntityDefinitions();

        var repo = new CustomEntityDefinitionRepository(entityDefinitions);
        var result = repo.GetByCode(definitionCode);

        result.CustomEntityDefinitionCode.Should().Be(definitionCode);
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
}
