Sometimes you may want to customize the data sent to your views by the dynamic page system. There are four types of view models Cofoundry uses for dynamic views:

- **IPageViewModel:** Used for most dynamic pages. The base type is `PageViewModel`.
- **ICustomEntityPageViewModel<TDisplayModel>:** Generic view model type used for [custom entity pages](Custom-Entity-Pages). Base type is `CustomEntityPageViewModel<TDisplayModel>`.
- **IErrorPageViewModel:** A simple model used for error pages. 
- **INotFoundPageViewModel:** A simple model used for the 404 page. 

To use a different model you will first need to create a class that implements one of the three view model interfaces or extend one the base classes.

#### ExamplePageViewModel.cs

In this example we extend the base `PageViewModel` class and add a simple string property. More commonly you may want to add global data like SEO or custom user data that can be used in your layout pages.

```csharp
using Cofoundry.Web;

public class ExamplePageViewModel : PageViewModel
{
    public string? ExampleMessage { get; set; }
}
```

To use this in your template views you'll need to override some of the base Cofoundry functionality. There's a couple of ways to do this depending on how much you want to customize the view models:

## Overriding the IPageViewModelFactory

If all you want to do is use a different view model type, the simplest thing to do is override the base `IPageViewModelFactory` implementation:

#### ExamplePageViewModelFactory.cs

In this example each of the three view model types have been given new implementations. If you only wanted to override one of them then you could instead inherit from `PageViewModelFactory` to take advantage of the base functionality.

```csharp
using Cofoundry.Web;

public class ExamplePageViewModelFactory : IPageViewModelFactory
{
    public IPageViewModel CreatePageViewModel()
    {
        return new ExamplePageViewModel();
    }
    
    public ICustomEntityPageViewModel<TDisplayModel> CreateCustomEntityPageViewModel<TDisplayModel>() where TDisplayModel : ICustomEntityPageDisplayModel
    {
        return new ExampleCustomEntityPageViewModel<TDisplayModel>();
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
```

#### ExampleDependencyRegistration.cs

To override the existing `IPageViewModelFactory` implementation we need to register the factory with the [Cofoundry DI framework](/framework/dependency-injection#overriding-registrations).

```csharp
using Cofoundry.Core.DependencyInjection;
using Cofoundry.Web;

public class ExampleDependencyRegistration : IDependencyRegistration
{
    public void Register(IContainerRegister container)
    {
        var overrideOptions = RegistrationOptions.Override();

        container.Register<IPageViewModelFactory, ExamplePageViewModelFactory>(overrideOptions);
    }
}
```

## Overriding IPageViewModelBuilder

Often you want to do more than just change the view model type for example adding some new data or altering the existing data in the model. To do this you can override `IPageViewModelBuilder` which gives you total control over the view model that is returned.

#### ExamplePageViewModelBuilder.cs

Here we're demonstrating implementing the `IPageViewModelBuilder` fully, but as is the case with `IPageViewModelFactory`, we could instead inherit from the base `PageViewModelBuilder` implementation if we didn't need to override every view model.

```csharp
using Cofoundry.Web;
using Cofoundry.Domain;

public class ExamplePageViewModelBuilder : IPageViewModelBuilder
{
    private readonly IPageViewModelFactory _pageViewModelFactory;
    private readonly IPageViewModelMapper _pageViewModelMapper;

    public ExamplePageViewModelBuilder(
        IPageViewModelFactory pageViewModelFactory,
        IPageViewModelMapper pageViewModelMapper
        )
    {
        // Constructor injection is supported
        // Here we make use of the same helpers used in the base class
        _pageViewModelFactory = pageViewModelFactory;
        _pageViewModelMapper = pageViewModelMapper;
    }

    public async Task<IPageViewModel> BuildPageViewModelAsync(PageViewModelBuilderParameters mappingParameters)
    {
        // Create the custom view model instance
        var viewModel = new ExamplePageViewModel();

        // Do the base mapping
        await _pageViewModelMapper.MapPageViewModelAsync(viewModel, mappingParameters);

        // TODO: insert your custom custom mapping
        viewModel.ExampleMessage = "I have a cunning plan.";
        
        return viewModel;
    }

    public async Task<ICustomEntityPageViewModel<TDisplayModel>> BuildCustomEntityPageViewModelAsync<TDisplayModel>(
        CustomEntityPageViewModelBuilderParameters mappingParameters
        ) where TDisplayModel : ICustomEntityPageDisplayModel
    {
        // Create the custom view model instance
        var viewModel = new ExampleCustomEntityPageViewModel<TDisplayModel>();

        // Do the base mapping
        await _pageViewModelMapper.MapCustomEntityViewModelAsync(viewModel, mappingParameters);

        // Example of calling an async custom mapping function
        await ExampleCustomMappingAsync(viewModel);

        return viewModel;
    }

    public async Task<IErrorPageViewModel> BuildErrorPageViewModelAsync(ErrorPageViewModelBuilderParameters mappingParameters)
    {
        // This example show using the default behaviour without any customization
        // You could alternatively inherit from PageViewModelBuilder and use the base implementation
        var viewModel = _pageViewModelFactory.CreateErrorPageViewModel();

        await _pageViewModelMapper.MapErrorPageViewModelAsync(viewModel, mappingParameters);

        return viewModel;
    }
    
    public async Task<INotFoundPageViewModel> BuildNotFoundPageViewModelAsync(NotFoundPageViewModelBuilderParameters mappingParameters)
    {
        // This example show using the default behaviour without any customization
        // You could alternatively inherit from PageViewModelBuilder and use the base implementation
        var viewModel = _pageViewModelFactory.CreateNotFoundPageViewModel();

        await _pageViewModelMapper.MapNotFoundPageViewModelAsync(viewModel, mappingParameters);

        return viewModel;
    }

    private static Task ExampleCustomMappingAsync<TDisplayModel>(ICustomEntityPageViewModel<TDisplayModel> model)
        where TDisplayModel : ICustomEntityPageDisplayModel
    {
        return Task.CompletedTask;
    }
}
```
