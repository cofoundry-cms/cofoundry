using Cofoundry.Core.ResourceFiles;
using Cofoundry.Domain.Internal;
using Cofoundry.Web;
using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Domain.Tests.Integration.Mocks
{
    /// <summary>
    /// Used during tests to add multiple mock page templates based on the same
    /// file to achieve isolated entities for testing.
    /// </summary>
    public class TestPageTemplateViewFileLocator : IPageTemplateViewFileLocator
    {
        private readonly PageTemplateViewFileLocator _pageTemplateViewFileLocator;
        private List<PageTemplateFile> _mockTemplates = new List<PageTemplateFile>();

        public TestPageTemplateViewFileLocator(
            IRazorViewEngine razorViewEngine,
            IResourceLocator resourceLocator,
            IEmptyActionContextFactory emptyActionContextFactory,
            IEnumerable<IPageTemplateViewLocationRegistration> pageTemplateViewLocationRegistrations
            )
        {
            _pageTemplateViewFileLocator = new PageTemplateViewFileLocator(razorViewEngine, resourceLocator, emptyActionContextFactory, pageTemplateViewLocationRegistrations);
        }

        public void AddMockTemplateFile(PageTemplateFile templateFile)
        {
            // Paths must be unique, so to avoid duplicates we have to fudge the filename, which in turn
            // is corrected in TestViewFileReader
            templateFile.VirtualPath += TestViewFileReader.MOCK_DATA_POSTFIX + templateFile.FileName;

            _mockTemplates.Add(templateFile);
        }

        public IEnumerable<PageTemplateFile> GetPageTemplateFiles(string searchText = null)
        {
            var matchingMockTemplates = _mockTemplates
                .Where(t => searchText == null || t.FileName.Contains(searchText, StringComparison.OrdinalIgnoreCase));

            return _pageTemplateViewFileLocator
                .GetPageTemplateFiles(searchText)
                .Union(matchingMockTemplates)
                .OrderBy(f => f.FileName);
        }

        public string ResolvePageTemplatePartialViewPath(string partialName)
        {
            return _pageTemplateViewFileLocator.ResolvePageTemplatePartialViewPath(partialName);
        }
    }
}
