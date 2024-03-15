using Cofoundry.Core.ResourceFiles;
using Cofoundry.Domain.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;

namespace Cofoundry.Web;

/// <summary>
/// Default (Web) implementation of <see cref="IPageTemplateViewFileLocator"/>.
/// </summary>
public class PageTemplateViewFileLocator : IPageTemplateViewFileLocator
{
    private readonly static char[] TEMPLATE_NAME_CHAR_TO_TRIM = ['_', '-'];
    private const string VIEW_FILE_EXTENSION = ".cshtml";

    private readonly IResourceLocator _resourceLocator;
    private readonly IRazorViewEngine _razorViewEngine;
    private readonly IEmptyActionContextFactory _emptyActionContextFactory;
    private readonly IEnumerable<IPageTemplateViewLocationRegistration> _pageTemplateViewLocationRegistrations;

    public PageTemplateViewFileLocator(
        IRazorViewEngine razorViewEngine,
        IResourceLocator resourceLocator,
        IEmptyActionContextFactory emptyActionContextFactory,
        IEnumerable<IPageTemplateViewLocationRegistration> pageTemplateViewLocationRegistrations
        )
    {
        _razorViewEngine = razorViewEngine;
        _resourceLocator = resourceLocator;
        _emptyActionContextFactory = emptyActionContextFactory;
        _pageTemplateViewLocationRegistrations = pageTemplateViewLocationRegistrations;
    }

    /// <inheritdoc/>
    public IEnumerable<PageTemplateFile> GetPageTemplateFiles(string? searchText = null)
    {
        return GetUnorderedPageTemplateFiles(searchText).OrderBy(l => l.FileName);
    }

    /// <inheritdoc/>
    public string? ResolvePageTemplatePartialViewPath(string? partialName)
    {
        if (partialName == null)
        {
            return null;
        }

        if (FileExists(partialName))
        {
            return partialName;
        }

        var view = _razorViewEngine.FindView(_emptyActionContextFactory.Create(), partialName, false);

        if (view.Success)
        {
            return view.View.Path;
        }

        return null;
    }

    private bool FileExists(string? path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }

        if (path[0] != '~' && path[0] != '/')
        {
            return false;
        }

        return _resourceLocator.FileExists(path);
    }

    private IEnumerable<PageTemplateFile> GetUnorderedPageTemplateFiles(string? searchText)
    {
        var templateDirecotryPaths = _pageTemplateViewLocationRegistrations.SelectMany(r => r.GetPathPrefixes());
        var templateDirectories = templateDirecotryPaths
            .Select(p => _resourceLocator.GetDirectory(p))
            .Where(d => d.Exists);

        foreach (var templateDirectoryPath in templateDirecotryPaths)
        {
            foreach (var layoutFile in SearchDirectoryForPageTemplateFiles(templateDirectoryPath, searchText))
            {
                yield return layoutFile;
            }
        }
    }

    private IEnumerable<PageTemplateFile> SearchDirectoryForPageTemplateFiles(string directoryPath, string? searchText)
    {
        var directoryContents = _resourceLocator.GetDirectory(directoryPath);

        foreach (var file in directoryContents.Where(f => !f.IsDirectory))
        {
            // filename contains the search text and is located in a 'PageTemplates' folder, but not a 'partials' folder and has the extension .cshtml
            if ((searchText == null || Contains(file.Name, searchText))
                && !Contains(file.Name, "_ViewStart")
                && !Contains(file.Name, "_ViewImports")
                && file.Name.EndsWith(VIEW_FILE_EXTENSION, StringComparison.OrdinalIgnoreCase))
            {
                yield return MapPageTemplateFile(directoryPath, file);
            }
        }

        foreach (var childDirectoryName in directoryContents
            .Where(f => f.IsDirectory)
            .Select(f => f.Name)
            )
        {
            var childDirectoryPath = RelativePathHelper.Combine(directoryPath, childDirectoryName);
            foreach (var file in SearchDirectoryForPageTemplateFiles(childDirectoryPath, searchText))
            {
                yield return file;
            }
        }
    }

    private static PageTemplateFile MapPageTemplateFile(string virtualDirectoryPath, IFileInfo file)
    {
        var fileName = Path.ChangeExtension(file.Name, null).TrimStart(TEMPLATE_NAME_CHAR_TO_TRIM);
        var virtualPath = RelativePathHelper.Combine(virtualDirectoryPath, file.Name);
        var templateFile = new PageTemplateFile()
        {
            FileName = fileName,
            VirtualPath = virtualPath
        };

        return templateFile;
    }

    /// <summary>
    /// Helper for readable case insensitive string comparing
    /// </summary>
    private static bool Contains(string compareFrom, string compareTo)
    {
        return string.IsNullOrWhiteSpace(compareFrom)
            || string.IsNullOrWhiteSpace(compareTo)
            || compareFrom.Contains(compareTo, StringComparison.OrdinalIgnoreCase);
    }
}
