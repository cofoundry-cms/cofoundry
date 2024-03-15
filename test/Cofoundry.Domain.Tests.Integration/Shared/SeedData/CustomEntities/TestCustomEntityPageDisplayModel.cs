namespace Cofoundry.Domain.Tests.Integration;

public class TestCustomEntityPageDisplayModel
    : TestCustomEntityDataModel
    , ICustomEntityPageDisplayModel<TestCustomEntityDataModel>
{
    public string PageTitle { get; set; } = string.Empty;
    public string? MetaDescription { get; set; }
}
