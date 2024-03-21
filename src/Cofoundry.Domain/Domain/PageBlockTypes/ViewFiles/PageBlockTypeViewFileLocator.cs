﻿using Cofoundry.Core.ResourceFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Allows searching for view files in the website directory.
/// </summary>
public class PageBlockTypeViewFileLocator : IPageBlockTypeViewFileLocator
{
    const string FILE_EXTENSION = ".cshtml";
    const string TEMPLATES_FOLDER_NAME = "Templates";
    const string PARTIALS_FOLDER_NAME = "Partials";

    private readonly IResourceLocator _resourceLocator;
    private readonly IEnumerable<IPageBlockTypeViewLocationRegistration> _pageBlockTypeViewLocationRegistrations;
    private readonly IPageBlockTypeCache _pageBlockTypeCache;
    private readonly ILogger<PageBlockTypeViewFileLocator> _logger;

    public PageBlockTypeViewFileLocator(
        IResourceLocator resourceLocator,
        IEnumerable<IPageBlockTypeViewLocationRegistration> pageBlockTypeViewLocationRegistrations,
        IPageBlockTypeCache pageBlockTypeCache,
        ILogger<PageBlockTypeViewFileLocator> logger
        )
    {
        _resourceLocator = resourceLocator;
        _pageBlockTypeViewLocationRegistrations = pageBlockTypeViewLocationRegistrations;
        _pageBlockTypeCache = pageBlockTypeCache;
        _logger = logger;
    }

    /// <summary>
    /// Returns the full path of a page block type view file using the file name, or
    /// null if it could not be found.
    /// </summary>
    /// <param name="pageBlockTypeFileName">The file name (without extension) of the page block type. E.g 'RawHtml', 'SingleLineText'</param>
    public virtual string? GetPathByFileName(string pageBlockTypeFileName)
    {
        var blockLocation = GetLocationByFileName(pageBlockTypeFileName);

        return blockLocation?.Path;
    }

    /// <summary>
    /// Gets the virtual path for a page block type template using the
    /// specified file name
    /// </summary>
    /// <param name="pageBlockTypeFileName">The file name of the page block type this template is for e.g. 'SingleLineText'</param>
    /// <param name="templateFileName">The file name of the template e.g. 'H1'</param>
    public virtual string? GetTemplatePathByTemplateFileName(string pageBlockTypeFileName, string templateFileName)
    {
        var blockTypeLocation = GetLocationByFileName(pageBlockTypeFileName);

        if (blockTypeLocation == null)
        {
            return null;
        }

        return blockTypeLocation.Templates.GetValueOrDefault(templateFileName)?.Path;
    }

    /// <summary>
    /// Gets a collection of paths to template files for the specified page block type.
    /// </summary>
    /// <param name="pageBlockTypeFileName">The file name of the page block e.g. 'SingleLineText'</param>
    public virtual IEnumerable<string> GetAllTemplatePathsByFileName(string pageBlockTypeFileName)
    {
        var blockTypeLocation = GetLocationByFileName(pageBlockTypeFileName);
        if (blockTypeLocation == null)
        {
            return Enumerable.Empty<string>();
        }

        return blockTypeLocation
            .Templates
            .Select(t => t.Value.Path);
    }

    private PageBlockTypeFileLocation? GetLocationByFileName(string fileName)
    {
        var cacheKey = FormatCacheKey(fileName);
        var blockTypeLocations = GetPageBlockTypeFileLocationsFromCache();
        var blockTypeLocation = blockTypeLocations.GetValueOrDefault(cacheKey);

        return blockTypeLocation;
    }

    private static string FormatCacheKey(string fileName)
    {
        // Make this case insensitive by converting to lowercase
        return fileName.ToLowerInvariant();
    }

    private IReadOnlyDictionary<string, PageBlockTypeFileLocation> GetPageBlockTypeFileLocationsFromCache()
    {
        var allBlockTypes = _pageBlockTypeCache.GetOrAddFileLocations(GetPageBlockTypeFileLocations);
        return allBlockTypes;
    }

    private Dictionary<string, PageBlockTypeFileLocation> GetPageBlockTypeFileLocations()
    {
        var viewDirectoryPaths = _pageBlockTypeViewLocationRegistrations
            .SelectMany(r => r.GetPathPrefixes())
            .Select(FormatViewFolder)
            .WhereNotNull()
            ;

        var templateFiles = new Dictionary<string, PageBlockTypeFileLocation>();
        var foldersToExclude = new string[] { TEMPLATES_FOLDER_NAME, PARTIALS_FOLDER_NAME };

        foreach (var directoryPath in viewDirectoryPaths)
        {
            var directory = _resourceLocator.GetDirectory(directoryPath);
            if (!directory.Exists)
            {
                continue;
            }

            foreach (var viewFile in FilterViewFiles(directory))
            {
                AddTemplateToDictionary(templateFiles, directoryPath, viewFile);
            }

            foreach (var childDirectory in directory
                .Where(d => d.IsDirectory
                    && !foldersToExclude.Any(f => d.Name.Equals(f, StringComparison.OrdinalIgnoreCase))))
            {
                var childDirectoryPath = RelativePathHelper.Combine(directoryPath, childDirectory.Name);

                foreach (var viewFile in FilterChildDirectoryFiles(childDirectoryPath, directory, foldersToExclude))
                {
                    AddTemplateToDictionary(templateFiles, childDirectoryPath, viewFile);
                }
            }
        }

        return templateFiles;
    }

    private void AddTemplateToDictionary(Dictionary<string, PageBlockTypeFileLocation> templateFiles, string directoryPath, IFileInfo viewFile)
    {
        var templateLocation = CreateTemplateFile(directoryPath, viewFile);
        var key = FormatCacheKey(templateLocation.FileName);
        var isUnique = !templateFiles.ContainsKey(key);

        if (isUnique)
        {
            templateFiles.Add(key, templateLocation);
        }
        else
        {
            _logger.LogWarning("Duplicate page block type template file '{FileName}' at location '{Path}'", templateLocation.FileName, templateLocation.Path);
        }
    }

    private IEnumerable<IFileInfo> FilterChildDirectoryFiles(string childDirectorPath, IDirectoryContents baseDirectory, string[] foldersToExclude)
    {
        var childDirectory = _resourceLocator.GetDirectory(childDirectorPath);

        foreach (var file in FilterViewFiles(childDirectory))
        {
            yield return file;
        }
    }

    private static IEnumerable<IFileInfo> FilterViewFiles(IDirectoryContents directory)
    {
        return directory
            .Where(f => !f.IsDirectory
                && !Contains(f.Name, "_ViewStart")
                && !Contains(f.Name, "_ViewImports")
                && f.Name.EndsWith(FILE_EXTENSION, StringComparison.OrdinalIgnoreCase));
    }

    private PageBlockTypeFileLocation CreateTemplateFile(string directoryPath, IFileInfo file)
    {
        var templateFile = new PageBlockTypeFileLocation
        {
            Path = RelativePathHelper.Combine(directoryPath, file.Name),
            FileName = Path.GetFileNameWithoutExtension(file.Name)
        };

        var templatePath = RelativePathHelper.Combine(directoryPath, TEMPLATES_FOLDER_NAME);

        var templateDirectory = _resourceLocator.GetDirectory(templatePath);
        if (templateDirectory != null)
        {
            templateFile.Templates = FilterViewFiles(templateDirectory)
                .GroupBy(t => t.Name, (k, v) => v.First()) // De-dup
                .Select(t => new PageBlockTypeTemplateFileLocation()
                {
                    FileName = Path.GetFileNameWithoutExtension(t.Name),
                    Path = RelativePathHelper.Combine(templatePath, t.Name)
                })
                .ToImmutableDictionary(t => t.FileName);
        }
        else
        {
            templateFile.Templates = ImmutableDictionary<string, PageBlockTypeTemplateFileLocation>.Empty;
        }

        return templateFile;
    }

    private static string? FormatViewFolder(string pathPrefix)
    {
        if (string.IsNullOrWhiteSpace(pathPrefix))
        {
            return null;
        }

        var path = "/" + pathPrefix.Trim('/') + "/";

        return path;
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