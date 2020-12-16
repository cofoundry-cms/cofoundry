using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Extracts information about a page block type from a view file with
    /// the specified name. If the file is not found then null is returned.
    /// </summary>
    public class GetPageBlockTypeFileDetailsByFileNameQueryHandler
        : IQueryHandler<GetPageBlockTypeFileDetailsByFileNameQuery, PageBlockTypeFileDetails>
        , IPermissionRestrictedQueryHandler<GetPageBlockTypeFileDetailsByFileNameQuery, PageBlockTypeFileDetails>
    {
        #region constructor

        const string TEMPLATE_NAME_FUNC = ".UseDisplayName";
        const string TEMPLATE_DESCRIPTION_FUNC = ".UseDescription";

        private readonly IPageBlockTypeViewFileLocator _viewLocator;
        private readonly IViewFileReader _viewFileReader;

        public GetPageBlockTypeFileDetailsByFileNameQueryHandler(
            IPageBlockTypeViewFileLocator viewLocator,
            IViewFileReader viewFileReader
            )
        {
            _viewLocator = viewLocator;
            _viewFileReader = viewFileReader;
        }

        #endregion

        #region execution

        public async Task<PageBlockTypeFileDetails> ExecuteAsync(GetPageBlockTypeFileDetailsByFileNameQuery query, IExecutionContext executionContext)
        {
            var viewPath = _viewLocator.GetPathByFileName(query.FileName);
            if (string.IsNullOrEmpty(viewPath))
            {
                throw new FileNotFoundException($"Page block type view file '{query.FileName}' not found.", query.FileName);
            }

            var view = await _viewFileReader.ReadViewFileAsync(viewPath);

            if (view == null)
            {
                throw new FileNotFoundException($"Page block type view file '{query.FileName}' not found at location '{viewPath}'.", viewPath);
            }

            var parsedData = ParseViewFile(view);
            var pageTemplateFileInfo = new PageBlockTypeFileDetails();

            pageTemplateFileInfo.Name = StringHelper.FirstNonEmpty(parsedData.Name, TextFormatter.PascalCaseToSentence(query.FileName));
            pageTemplateFileInfo.Description = parsedData.Description;
            pageTemplateFileInfo.Templates = await MapChildTemplates(query.FileName);

            return pageTemplateFileInfo;
        }

        private async Task<ICollection<PageBlockTypeTemplateFileDetails>> MapChildTemplates(string blockTypeFileName)
        {
            var templatePaths = _viewLocator.GetAllTemplatePathsByFileName(blockTypeFileName);

            if (EnumerableHelper.IsNullOrEmpty(templatePaths))
            {
                return Array.Empty<PageBlockTypeTemplateFileDetails>();
            }

            var templateFileDetails = new List<PageBlockTypeTemplateFileDetails>(templatePaths.Count());

            foreach (var templatePath in templatePaths)
            {
                var templateView = await _viewFileReader.ReadViewFileAsync(templatePath);
                if (templateView == null) continue;

                var templateViewData = ParseViewFile(templateView);
                templateViewData.FileName = Path.GetFileNameWithoutExtension(templatePath);
                if (string.IsNullOrWhiteSpace(templateViewData.Name))
                {
                    templateViewData.Name = TextFormatter.PascalCaseToSentence(templateViewData.FileName);
                }

                templateFileDetails.Add(templateViewData);
            }

            return templateFileDetails;
        }

        private PageBlockTypeTemplateFileDetails ParseViewFile(string viewFile)
        {
            var fileDetails = new PageBlockTypeTemplateFileDetails();

            using (var sr = new StringReader(viewFile))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains(TEMPLATE_NAME_FUNC))
                    {
                        fileDetails.Name = ParseFunctionParameter(line, TEMPLATE_NAME_FUNC);
                    }
                    else if (line.Contains(TEMPLATE_DESCRIPTION_FUNC))
                    {
                        fileDetails.Description = ParseFunctionParameter(line, TEMPLATE_DESCRIPTION_FUNC);
                    }

                    if (fileDetails.Name != null && fileDetails.Description != null)
                    {
                        break;
                    }
                }
            }

            return fileDetails;
        }
        
        private string ParseFunctionParameter(string textLine, string functionName)
        {
            var startFunc = functionName + "(\"";

            int start = textLine.IndexOf(startFunc) + startFunc.Length;
            var parameterValue = textLine.Substring(start);
            parameterValue = parameterValue.Substring(0, parameterValue.IndexOf('"'));

            return parameterValue;
        }

        #endregion

        #region Permission

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageBlockTypeFileDetailsByFileNameQuery query)
        {
            // Permissions are tied to the page templating system
            yield return new PageTemplateCreatePermission();
        }

        #endregion
    }
}
