namespace Cofoundry.Domain.Tests.Integration
{
    public class TestCustomEntityPageDisplayModel 
        : TestCustomEntityDataModel
        , ICustomEntityPageDisplayModel<TestCustomEntityDataModel>
    {
        public string PageTitle { get; set; }
        public string MetaDescription { get; set; }
    }
}