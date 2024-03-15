using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Tests.Data.UnstructuredData;

public class EmptyDataModelFactoryTests
{
    [Fact]
    public void WhenValid_Creates()
    {
        var factory = new EmptyDataModelFactory();
        var model = factory.Create<ICustomEntityDataModel>(typeof(ExampleModel));
        var typedModel = model as ExampleModel;

        using (new AssertionScope())
        {
            model.Should().NotBeNull();
            typedModel.Should().NotBeNull();
            typedModel?.ExampleProp.Should().Be(43);
        }
    }

    [Fact]
    public void WhenNoConstructor_Throws()
    {
        var factory = new EmptyDataModelFactory();
        var model = factory
            .Invoking(f => f.Create<ICustomEntityDataModel>(typeof(ExampleModelWithoutConstructor)))
            .Should()
            .Throw<Exception>()
            .WithMessage($"*Unable to create an empty instance*{nameof(ExampleModelWithoutConstructor)}*parameterless constructor*")
            .WithInnerException<MissingMethodException>();
    }

    [Fact]
    public void WhenConcreteTypeNotImplementsInterface_Throws()
    {
        var factory = new EmptyDataModelFactory();
        var model = factory
            .Invoking(f => f.Create<ICustomEntityDataModel>(typeof(ExampleModelNotImplementsInterface)))
            .Should()
            .Throw<Exception>()
            .WithMessage($"*Cannot create an empty instance*does not implement*{nameof(ICustomEntityDataModel)}*");
    }

    private class ExampleModel : ICustomEntityDataModel
    {
        public int ExampleProp { get; set; } = 43;
    }

    private class ExampleModelNotImplementsInterface
    {
    }

    private class ExampleModelWithoutConstructor : ICustomEntityDataModel
    {
        public ExampleModelWithoutConstructor(int value)
        {
            ExampleProp = value;
        }

        public int ExampleProp { get; set; }
    }
}
