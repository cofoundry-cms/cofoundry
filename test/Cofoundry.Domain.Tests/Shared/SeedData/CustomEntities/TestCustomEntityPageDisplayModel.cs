namespace Cofoundry.Domain.Tests;

public class TestCustomEntityPageDisplayModel
    : TestCustomEntityDataModel
    , ICustomEntityPageDisplayModel<TestCustomEntityDataModel>
{
    public string PageTitle { get; set; } = string.Empty;
    public string? MetaDescription { get; set; }
}
