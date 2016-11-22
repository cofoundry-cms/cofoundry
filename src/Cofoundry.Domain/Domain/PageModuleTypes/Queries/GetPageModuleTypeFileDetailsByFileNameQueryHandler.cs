using AutoMapper;
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

namespace Cofoundry.Domain
{
    /// <summary>
    /// Extracts information about a page module from a view file with
    /// the specified name. If the file is not found then null is returned.
    /// </summary>
    public class GetPageModuleTypeFileDetailsByFileNameQueryHandler
        : IAsyncQueryHandler<GetPageModuleTypeFileDetailsByFileNameQuery, PageModuleTypeFileDetails>
        , IPermissionRestrictedQueryHandler<GetPageModuleTypeFileDetailsByFileNameQuery, PageModuleTypeFileDetails>
    {
        #region constructor

        const string TEMPLATE_NAME_FUNC = ".UseDisplayName";
        const string TEMPLATE_DESCRIPTION_FUNC = ".UseDescription";

        private readonly IPageModuleTypeViewFileLocator _viewLocator;
        private readonly IViewFileReader _viewFileReader;

        public GetPageModuleTypeFileDetailsByFileNameQueryHandler(
            IPageModuleTypeViewFileLocator viewLocator,
            IViewFileReader viewFileReader
            )
        {
            _viewLocator = viewLocator;
            _viewFileReader = viewFileReader;
        }

        #endregion

        #region execution

        public async Task<PageModuleTypeFileDetails> ExecuteAsync(GetPageModuleTypeFileDetailsByFileNameQuery query, IExecutionContext executionContext)
        {
            var viewPath = _viewLocator.GetPathByFileName(query.FileName);
            var view = await _viewFileReader.ReadViewFileAsync(viewPath);

            if (view == null)
            {
                throw new ApplicationException("View file not found: " + query.FileName);
            }

            var parsedData = ParseViewFile(view);
            var pageTemplateFileInfo = new PageModuleTypeFileDetails();

            pageTemplateFileInfo.Name = StringHelper.FirstNonEmpty(parsedData.Name, TextFormatter.PascalCaseToSentence(query.FileName));
            pageTemplateFileInfo.Description = parsedData.Description;
            pageTemplateFileInfo.Templates = await MapChildTemplates(query.FileName);

            return pageTemplateFileInfo;
        }

        private async Task<IEnumerable<PageModuleTypeTemplateFileDetails>> MapChildTemplates(string moduleFileName)
        {
            var templatePaths = _viewLocator.GetAllTemplatePathByModuleFileName(moduleFileName);

            if (EnumerableHelper.IsNullOrEmpty(templatePaths))
            {
                return Enumerable.Empty<PageModuleTypeTemplateFileDetails>();
            }

            var templateFileDetails = new List<PageModuleTypeTemplateFileDetails>(templatePaths.Count());

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

        private PageModuleTypeTemplateFileDetails ParseViewFile(string viewFile)
        {
            var fileDetails = new PageModuleTypeTemplateFileDetails();

            using (var sr = new StringReader(viewFile))
            {
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains(TEMPLATE_NAME_FUNC))
                    {
                        fileDetails.Name = ParseFunctionParameter(line, TEMPLATE_DESCRIPTION_FUNC);
                    }
                    else if (line.Contains(TEMPLATE_DESCRIPTION_FUNC))
                    {
                        fileDetails.Description = ParseFunctionParameter(line, TEMPLATE_DESCRIPTION_FUNC);
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

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageModuleTypeFileDetailsByFileNameQuery query)
        {
            // Permissions are tied to the page templating system
            yield return new PageTemplateCreatePermission();
        }

        #endregion
    }
}
