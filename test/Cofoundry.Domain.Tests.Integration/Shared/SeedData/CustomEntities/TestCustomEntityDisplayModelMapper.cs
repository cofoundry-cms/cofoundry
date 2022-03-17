namespace Cofoundry.Domain.Tests.Integration;

public class TestCustomEntityDisplayModelMapper
    : ICustomEntityDisplayModelMapper<TestCustomEntityDataModel, TestCustomEntityPageDisplayModel>
{
    public Task<TestCustomEntityPageDisplayModel> MapDisplayModelAsync(
        CustomEntityRenderDetails renderDetails,
        TestCustomEntityDataModel dataModel,
        PublishStatusQuery publishStatusQuery
        )
    {
        var displayModel = new TestCustomEntityPageDisplayModel()
        {
            PageTitle = renderDetails.Title,
            MetaDescription = "Test Meta Description"
        };

        return Task.FromResult(displayModel);
    }
}
