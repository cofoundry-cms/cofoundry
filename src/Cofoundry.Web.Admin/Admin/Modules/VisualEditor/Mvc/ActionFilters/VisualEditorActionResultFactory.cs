using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using Cofoundry.Core.ResourceFiles;
using Cofoundry.Core.Json;
using Cofoundry.Domain;

namespace Cofoundry.Web.Admin
{
    public class VisualEditorActionResultFactory : IVisualEditorActionResultFactory
    {
        private readonly IStaticResourceReferenceRenderer _staticResourceReferenceRenderer;
        private readonly IAdminRouteLibrary _adminRouteLibrary;
        private readonly IPageResponseDataCache _pageResponseDataCache;
        private readonly IJsonSerializerSettingsFactory _jsonSerializerSettingsFactory;
        private readonly IRazorViewRenderer _razorViewRenderer;
        private readonly IResourceLocator _resourceLocator;

        public VisualEditorActionResultFactory(
            IStaticResourceReferenceRenderer staticResourceReferenceRenderer,
            IAdminRouteLibrary adminRouteLibrary,
            IPageResponseDataCache pageResponseDataCache,
            IJsonSerializerSettingsFactory jsonSerializerSettingsFactory,
            IRazorViewRenderer razorViewRenderer,
            IResourceLocator resourceLocator
            )
        {
            _staticResourceReferenceRenderer = staticResourceReferenceRenderer;
            _adminRouteLibrary = adminRouteLibrary;
            _pageResponseDataCache = pageResponseDataCache;
            _jsonSerializerSettingsFactory = jsonSerializerSettingsFactory;
            _resourceLocator = resourceLocator;
            _razorViewRenderer = razorViewRenderer;
        }

        public IActionResult Create(IActionResult wrappedActionResult)
        {
            return new VisualEditorActionResult(
                wrappedActionResult,
                _staticResourceReferenceRenderer,
                _adminRouteLibrary,
                _jsonSerializerSettingsFactory,
                _pageResponseDataCache,
                _razorViewRenderer,
                _resourceLocator
                );
        }
    }
}