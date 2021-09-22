using Cofoundry.Domain;
using Cofoundry.Web;

namespace Cofoundry.BasicTestSite
{
    public class ExampleCustomEntityPageViewModel<TModel> : CustomEntityPageViewModel<TModel>
        where TModel : ICustomEntityPageDisplayModel
    {
        public string TestMessage { get; set; }
    }
}