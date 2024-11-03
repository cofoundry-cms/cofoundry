Cofoundry automatically bootstraps the ASP.NET Core error handling middleware to provide you with friendly error pages out of the box for exceptions and for other error status codes such as 404s. 

## The Developer Exception Page

By default, when running in the development environment, Cofoundry will automatically add the [developer exception page](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling#the-developer-exception-page) to give you detailed information on exceptions.

This behavior can be overridden using the `Cofoundry:Debug:DeveloperExceptionPageMode` setting, which can have the values *"DevelopmentOnly"*, *"On"* or *"Off"*.

**Example appsettings.json**

```json
{
  "Cofoundry": {
    "Debug": {
      "DeveloperExceptionPageMode": "On"
    }
  }
}
```

## Customizing the Error Page

By default the built-in error page will use your default layout page to render in, but typically you'll want to override this and have your own page with custom text and styles.

Cofoundry lets you customize a handful of different error pages. Each one should be placed in your **'Views/Shared'** folder, where it will be automatically discovered.

#### Generic Error

Unless you specify other error view files, all errors will display the generic error page. To override this built-in view you should create a file named **Error.cshtml**, which can use the `IErrorPageViewModel` as the model.

```html
@model IErrorPageViewModel

<div class="container">

    <h1>Error: @Model.StatusCodeDescription</h1>

    <p>Uh oh! There's been an error!</p>

</div>

```

#### Not Found (404)

By default 404 errors are displayed using the generic error view, but often you'll want to create a specific view for this common type of error. To do this create a file named **NotFound.cshtml**, which can use  `INotFoundPageViewModel` as the model.

```html
@model INotFoundPageViewModel

<h1>Not Found</h1>

<p>
    Sorry, the page <strong>@Model.Path</strong> could not be found
</p>

```

In your controllers you can also take advantage of the `INotFoundViewHelper` to return a 404 result. This has the added benefit of checking for [Rewrite Rules](rewrite-rules) and automatically redirecting:

```csharp
using Cofoundry.Web;

public class ProductController : Controller
{
    private readonly INotFoundViewHelper _notFoundViewHelper;
    private readonly IProductRepository _productRepository;

    public ProductController(
        INotFoundViewHelper notFoundViewHelper,
        IProductRepository productRepository
        )
    {
        _notFoundViewHelper = notFoundViewHelper;
        _productRepository = productRepository;
    }

    public async Task<ActionResult> Details(int id)
    {
        var product = _productRepository.GetById(id);

        if (product == null)
        {
            return await _notFoundViewHelper.GetViewAsync(this);
        }

        return View(product);
    }
}
```

#### Other Status Codes

You can add custom error pages for other http status codes by creating a file using the status code as the name, e.g. **400.cshtml** or **418.cshtml** which can use `IErrorPageViewModel` as the model.



