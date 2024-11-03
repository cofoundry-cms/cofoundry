*Cofoundry currently supports only basic customization of the admin panel dashboard, allowing you to replace the content with custom html. We intend to develop more advanced features in a future release, so if you want to contribute suggestions please check out [Issue #150](https://github.com/cofoundry-cms/cofoundry/issues/150).*

## Replacing the default content

To replace the default dashboard content simply add an html file to the following path in your project: `/Cofoundry/Admin/Dashboard/Dashboard.html`.

This html file replaces the content area of the dashboard, so for example you could replace the default content with an introduction and some quick links:

**Dashboard.html**

```html
<p>
    Welcome to the admin panel for our site.
    Here's some links to get you started:
</p>
<ul>
    <li><a href="/admin/pages#/new">Add a page</a></li>
    <li><a href="/admin/images#/new">Add an image</a></li>
    <li><a href="/admin/products">View products</a></li>
</ul>
```

## IDashboardContentProvider

For complete control over the html content that gets added you can override the default `IDashboardContentProvider` implementation using hte [DI system](/framework/dependency-injection#overriding-registrations).

```csharp
using Cofoundry.Domain;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;

public class ExampleDashboardContentProvider : IDashboardContentProvider
{
    private readonly IContentRepository _contentRepository;
        
    public ExampleDashboardContentProvider(
        IContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    public async Task<IHtmlContent> GetAsync()
    {        
        var user = await _contentRepository
            .Users()
            .Current()
            .Get()
            .AsDetails()
            .ExecuteAsync();
        EntityNotFoundException.ThrowIfNull(user, "current");

        // Just some examples of changing the content depending on user properties
        // You could also load in an external file using IResourceLocator, which is
        // what the default provider does.
        if (user.Role.IsSuperAdminRole)
        {
            return new HtmlString("<h2>Hello Super Admin!</h2>"); 
        }

        return new HtmlString($"<h2>Hello { user.Username }!</h2>"); 
    }
}
```

