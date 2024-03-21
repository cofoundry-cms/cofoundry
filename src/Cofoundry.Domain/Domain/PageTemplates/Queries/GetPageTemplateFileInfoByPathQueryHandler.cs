using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Cofoundry.Domain.Internal;

public partial class GetPageTemplateFileInfoByPathQueryHandler
    : IQueryHandler<GetPageTemplateFileInfoByPathQuery, PageTemplateFileInfo>
    , IPermissionRestrictedQueryHandler<GetPageTemplateFileInfoByPathQuery, PageTemplateFileInfo>
{
    const string FUNC_OPENER = "(\"";
    const string REGION_FUNC = ".Template.Region";
    const string CUSTOM_ENTITY_REGION_FUNC = ".Template.CustomEntityRegion";
    const string TEMPLATE_DESCRIPTION_FUNC = ".Template.UseDescription";

    const string COMMENTS_REGEX = @"(@\*.*?(\*@))";

    private readonly IQueryExecutor _queryExecutor;
    private readonly IPageTemplateViewFileLocator _viewLocator;
    private readonly IViewFileReader _viewFileReader;
    private readonly IPageTemplateCustomEntityTypeMapper _pageTemplateCustomEntityTypeMapper;
    private readonly ILogger<GetPageTemplateFileInfoByPathQueryHandler> _logger;

    public GetPageTemplateFileInfoByPathQueryHandler(
        IPageTemplateViewFileLocator viewLocator,
        IQueryExecutor queryExecutor,
        IViewFileReader viewFileReader,
        IPageTemplateCustomEntityTypeMapper pageTemplateCustomEntityTypeMapper,
        ILogger<GetPageTemplateFileInfoByPathQueryHandler> logger
        )
    {
        _queryExecutor = queryExecutor;
        _viewLocator = viewLocator;
        _pageTemplateCustomEntityTypeMapper = pageTemplateCustomEntityTypeMapper;
        _logger = logger;
        _viewFileReader = viewFileReader;
    }

    public async Task<PageTemplateFileInfo> ExecuteAsync(GetPageTemplateFileInfoByPathQuery query, IExecutionContext executionContext)
    {
        var view = await _viewFileReader.ReadViewFileAsync(query.FullPath);

        if (view == null)
        {
            throw new FileNotFoundException("View file not found: " + query.FullPath);
        }

        var pageTemplateFileInfo = await ParseViewFile(view, true, executionContext);

        return pageTemplateFileInfo;
    }

    /// <summary>
    /// Here we run through each line of the view file and try and work out 
    /// what the regions are and extract a few other bits of imformation.
    /// </summary>
    private async Task<PageTemplateFileInfo> ParseViewFile(string viewFile, bool isRootFile, IExecutionContext executionContext)
    {
        viewFile = PrepareViewFileForParsing(viewFile);

        var pageTemplateFileInfo = new PageTemplateFileInfo
        {
            PageType = PageType.Generic
        };

        var regions = new List<PageTemplateFileRegion>();

        using var sr = new StringReader(viewFile);
        string? line;
        var parseCustomModelType = isRootFile;

        while ((line = sr.ReadLine()) != null)
        {
            if (parseCustomModelType)
            {
                // Check for model type on first line only
                await SetCustomModelTypeFieldsAsync(pageTemplateFileInfo, line, executionContext);
                if (pageTemplateFileInfo.CustomEntityDefinition != null)
                {
                    parseCustomModelType = false;
                }
            }

            if (line.Contains(REGION_FUNC + FUNC_OPENER))
            {
                var regionName = ParseFunctionParameter(line, REGION_FUNC);
                regions.Add(new PageTemplateFileRegion() { Name = regionName });
            }
            else if (line.Contains(CUSTOM_ENTITY_REGION_FUNC + FUNC_OPENER))
            {
                var regionName = ParseFunctionParameter(line, CUSTOM_ENTITY_REGION_FUNC);
                regions.Add(new PageTemplateFileRegion() { Name = regionName, IsCustomEntityRegion = true });
            }
            else if (line.Contains(TEMPLATE_DESCRIPTION_FUNC + FUNC_OPENER))
            {
                pageTemplateFileInfo.Description = ParseFunctionParameter(line, TEMPLATE_DESCRIPTION_FUNC);
            }
            else
            {
                var partialMatch = PartialNameRegex().Match(line);

                if (partialMatch.Success)
                {
                    var partialName = partialMatch.Groups[1].Value;
                    var partialRegions = await ParsePartialView(partialName, executionContext);
                    regions.AddRange(partialRegions);
                }
            }
        }

        pageTemplateFileInfo.Regions = regions.ToArray();

        return pageTemplateFileInfo;
    }

    private static string? TrimLineAndRemoveComments(string? line)
    {
        if (line == null)
        {
            return line;
        }

        return CommentsRegex().Replace(line, string.Empty);
    }

    /// <summary>
    /// Removes commands and whitespace in region definitions defined in 
    /// a fluent manner across multiple code lines.
    /// </summary>
    private string PrepareViewFileForParsing(string viewFile)
    {
        var whitespaceRemoved = RemoveMethodWhitespaceRegex().Replace(viewFile, RemoveWhitespace);
        var commentsRemoved = CommentsRegexSingleLine().Replace(whitespaceRemoved, string.Empty);

        return commentsRemoved;
    }

    private string RemoveWhitespace(Match e)
    {
        return TrimWhitespaceRegex().Replace(e.Value, string.Empty);
    }

    private async Task SetCustomModelTypeFieldsAsync(
        PageTemplateFileInfo pageTemplateFileInfo,
        string line,
        IExecutionContext executionContext
        )
    {
        var match = CustomEntityModelRegex().Match(line);
        if (match.Success)
        {
            // Try and find a matching custom entity model type.
            var modelType = _pageTemplateCustomEntityTypeMapper.Map(match.Groups[1].Value);

            if (modelType == null)
            {
                return;
            }

            var query = new GetCustomEntityDefinitionMicroSummaryByDisplayModelTypeQuery
            {
                DisplayModelType = modelType
            };

            pageTemplateFileInfo.CustomEntityDefinition = await _queryExecutor.ExecuteAsync(query, executionContext);
            EntityNotFoundException.ThrowIfNull(pageTemplateFileInfo.CustomEntityDefinition, modelType);
            pageTemplateFileInfo.CustomEntityModelType = match.Groups[1].Value;

            pageTemplateFileInfo.PageType = PageType.CustomEntityDetails;
        }
    }

    private async Task<IEnumerable<PageTemplateFileRegion>> ParsePartialView(string partialName, IExecutionContext executionContext)
    {
        var partialPath = _viewLocator.ResolvePageTemplatePartialViewPath(partialName);

        if (string.IsNullOrEmpty(partialPath))
        {
            _logger.LogWarning("Could not find partial view file '{PartialName}'", partialName);
            return Enumerable.Empty<PageTemplateFileRegion>();
        }

        var partialFile = await _viewFileReader.ReadViewFileAsync(partialPath);
        if (partialFile == null)
        {
            _logger.LogWarning("Could not read partial view file '{PartialName}'", partialName);
            return Enumerable.Empty<PageTemplateFileRegion>();
        }

        var templateFileInfo = await ParseViewFile(partialFile, false, executionContext);

        return templateFileInfo.Regions;
    }

    private static string ParseFunctionParameter(string textLine, string functionName)
    {
        var startFunc = functionName + FUNC_OPENER;

        var start = textLine.IndexOf(startFunc) + startFunc.Length;
        var parameterValue = textLine.Substring(start);
        parameterValue = parameterValue.Substring(0, parameterValue.IndexOf('"'));

        return parameterValue;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetPageTemplateFileInfoByPathQuery query)
    {
        yield return new CompositePermissionApplication(new PageTemplateCreatePermission(), new PageTemplateUpdatePermission());
    }

    [GeneratedRegex("Html.(?:Render)?Partial(?:Async)?\\(\"([^\"]+)\"")]
    private static partial Regex PartialNameRegex();

    /// <summary>
    /// This regex find models with generic parameters and captures the last generic type
    /// This could be the custom entity display model type and may have a namespace prefix
    /// </summary>
    [GeneratedRegex(@"\s*@(?:inherits|model)\s+[\w+<.]*<([\w\.]+)")]
    private static partial Regex CustomEntityModelRegex();

    [GeneratedRegex(COMMENTS_REGEX)]
    private static partial Regex CommentsRegex();

    [GeneratedRegex(COMMENTS_REGEX, RegexOptions.Singleline)]
    private static partial Regex CommentsRegexSingleLine();

    [GeneratedRegex(@"(\w+\s*\.Template+\s*\.[A-Za-z]*Region\()")]
    private static partial Regex RemoveMethodWhitespaceRegex();

    [GeneratedRegex(@"\s+")]
    private static partial Regex TrimWhitespaceRegex();
}