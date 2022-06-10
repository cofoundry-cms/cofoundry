using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;

namespace Cofoundry.Web;

/// <summary>
/// Extends the MvcRazorRuntimeCompilationOptions configuration adding Cofoundry specific
/// settings such as extending the FileProviders collection using IResourceFileProviderFactory.
/// </summary>
public class CofoundryMvcRazorRuntimeCompilationOptionsConfiguration : IMvcRazorRuntimeCompilationOptionsConfiguration
{
    private readonly IResourceFileProviderFactory _resourceFileProviderFactory;

    public CofoundryMvcRazorRuntimeCompilationOptionsConfiguration(
        IResourceFileProviderFactory resourceFileProviderFactory
        )
    {
        _resourceFileProviderFactory = resourceFileProviderFactory;
    }

    public void Configure(MvcRazorRuntimeCompilationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        options.FileProviders.Add(_resourceFileProviderFactory.Create());
    }
}
