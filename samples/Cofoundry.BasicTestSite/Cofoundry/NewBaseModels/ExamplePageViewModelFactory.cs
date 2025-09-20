namespace Cofoundry.BasicTestSite;

public class ExamplePageViewModelFactory : IPageViewModelFactory
{
    public ICustomEntityPageViewModel<TDisplayModel> CreateCustomEntityPageViewModel<TDisplayModel>() where TDisplayModel : ICustomEntityPageDisplayModel
    {
        return new ExampleCustomEntityPageViewModel<TDisplayModel>();
    }

    public IPageViewModel CreatePageViewModel()
    {
        return new ExamplePageViewModel();
    }

    public INotFoundPageViewModel CreateNotFoundPageViewModel()
    {
        return new ExampleNotFoundPageViewModel();
    }

    public IErrorPageViewModel CreateErrorPageViewModel()
    {
        return new ExampleErrorPageViewModel();
    }
}
