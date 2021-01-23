using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    public class GetPageTemplateFileInfoByPathQueryHandler 
        : IQueryHandler<GetPageTemplateFileInfoByPathQuery, PageTemplateFileInfo>
        , IPermissionRestrictedQueryHandler<GetPageTemplateFileInfoByPathQuery, PageTemplateFileInfo>
    {
        #region constructor

        const string FUNC_OPENER = "(\"";
        const string REGION_FUNC = ".Template.Region";
        const string CUSTOM_ENTITY_REGION_FUNC = ".Template.CustomEntityRegion";
        const string TEMPLATE_DESCRIPTION_FUNC = ".Template.UseDescription";

        const string REGEX_REMOVE_METHOD_WHITEPSPACE = @"(\w+\s*\.Template+\s*\.[A-Za-z]*Region\()";
        const string PARTIAL_NAME_REGEX = "Html.(?:Render)?Partial(?:Async)?\\(\"([^\"]+)\"";
        const string COMMENTS_REGEX = @"(@\*.*(\*@))";

        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageTemplateViewFileLocator _viewLocator;
        private readonly IViewFileReader _viewFileReader;
        private readonly IPageTemplateCustomEntityTypeMapper _pageTemplateCustomEntityTypeMapper;

        public GetPageTemplateFileInfoByPathQueryHandler(
            IPageTemplateViewFileLocator viewLocator,
            IQueryExecutor queryExecutor,
            IViewFileReader viewFileReader,
            IPageTemplateCustomEntityTypeMapper pageTemplateCustomEntityTypeMapper
            )
        {
            _queryExecutor = queryExecutor;
            _viewLocator = viewLocator;
            _pageTemplateCustomEntityTypeMapper = pageTemplateCustomEntityTypeMapper;
            _viewFileReader = viewFileReader;
        }

        #endregion

        #region execution

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

            var pageTemplateFileInfo = new PageTemplateFileInfo();
            pageTemplateFileInfo.PageType = PageType.Generic;

            var regions = new List<PageTemplateFileRegion>();

            using (var sr = new StringReader(viewFile))
            {
                string line;
                bool parseCustomModelType = isRootFile;

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
                        var partialMatch = Regex.Match(line, PARTIAL_NAME_REGEX);

                        if (partialMatch.Success)
                        {
                            var partialName = partialMatch.Groups[1].Value;
                            var partialRegions = await ParsePartialView(partialName, executionContext);
                            regions.AddRange(partialRegions);
                        }
                    }
                }
            }

            pageTemplateFileInfo.Regions = regions.ToArray();

            return pageTemplateFileInfo;
        }

        private string TrimLineAndRemoveComments(string line)
        {
            if (line == null) return line;

            return Regex.Replace(line, COMMENTS_REGEX, string.Empty);
        }

        /// <summary>
        /// Removes commands and whitespace in region definitions defined in 
        /// a fluent manner across multiple code lines.
        /// </summary>
        private string PrepareViewFileForParsing(string viewFile)
        {
            var whitespaceRemoved = Regex.Replace(viewFile, REGEX_REMOVE_METHOD_WHITEPSPACE, RemoveWhitepace);
            var commentsRemoved = Regex.Replace(whitespaceRemoved, COMMENTS_REGEX, string.Empty, RegexOptions.Singleline);

            return commentsRemoved;
        }

        private string RemoveWhitepace(Match e)
        {
            const string REGEX_TRIM_WHITESPACE = @"\s+";
            return Regex.Replace(e.Value, REGEX_TRIM_WHITESPACE, string.Empty);
        }

        private async Task SetCustomModelTypeFieldsAsync(
            PageTemplateFileInfo pageTemplateFileInfo, 
            string line, 
            IExecutionContext executionContext
            )
        {
            // This regex find models with generic parameters and captures the last generic type
            // This could be the custom entity display model type and may have a namespace prefix
            const string CUSTOM_ENTITY_MODEL_REGEX = @"\s*@(?:inherits|model)\s+[\w+<.]*<([\w\.]+)";

            var match = Regex.Match(line, CUSTOM_ENTITY_MODEL_REGEX);
            if (match.Success)
            {
                // Try and find a matching custom entity model type.
                var modelType = _pageTemplateCustomEntityTypeMapper.Map(match.Groups[1].Value);
                
                if (modelType == null) return;

                var query = new GetCustomEntityDefinitionMicroSummaryByDisplayModelTypeQuery();
                query.DisplayModelType = modelType;

                pageTemplateFileInfo.CustomEntityDefinition = await _queryExecutor.ExecuteAsync(query, executionContext);
                EntityNotFoundException.ThrowIfNull(pageTemplateFileInfo.CustomEntityDefinition, modelType);
                pageTemplateFileInfo.CustomEntityModelType = match.Groups[1].Value;

                pageTemplateFileInfo.PageType = PageType.CustomEntityDetails;
            }
        }

        private async Task<IEnumerable<PageTemplateFileRegion>> ParsePartialView(string partialName, IExecutionContext executionContext)
        {
            var partialPath = _viewLocator.ResolvePageTemplatePartialViewPath(partialName);

            Debug.Assert(!string.IsNullOrEmpty(partialPath), "Partial View file not found: " + partialName);

            if (string.IsNullOrEmpty(partialPath)) return Enumerable.Empty<PageTemplateFileRegion>();
            var partialFile = await _viewFileReader.ReadViewFileAsync(partialPath);

            return (await ParseViewFile(partialFile, false, executionContext)).Regions;
        }
        
        private string ParseFunctionParameter(string textLine, string functionName)
        {
            var startFunc = functionName + FUNC_OPENER;

            int start = textLine.IndexOf(startFunc) + startFunc.Length;
            var parameterValue = textLine.Substring(start);
            parameterValue = parameterValue.Substring(0, parameterValue.IndexOf('"'));

            return parameterValue;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageTemplateFileInfoByPathQuery query)
        {
            yield return new CompositePermissionApplication(new PageTemplateCreatePermission(), new PageTemplateUpdatePermission());
        }

        #endregion
    }
}
