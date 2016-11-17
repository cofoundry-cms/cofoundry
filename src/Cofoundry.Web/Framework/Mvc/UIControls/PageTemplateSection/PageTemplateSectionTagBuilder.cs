using Conditions;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Cofoundry.Domain;
using Cofoundry.Core.Web;

namespace Cofoundry.Web
{
    /// <summary>
    /// An object for building up a section tag using method chaining
    /// </summary>
    public class PageTemplateSectionTagBuilder : IHtmlString, IPageTemplateSectionTagBuilder
    {
        #region constructor

        const string DEFAULT_TAG ="div";
        private readonly IEditablePageViewModel _pageViewModel;
        private readonly HtmlHelper _htmlHelper;
        private readonly IModuleRenderer _moduleRenderer;

        public PageTemplateSectionTagBuilder(
            IModuleRenderer moduleRenderer,
            HtmlHelper htmlHelper,
            IEditablePageViewModel pageViewModel, 
            string sectionName)
        {
            Condition.Requires(sectionName).IsNotNullOrWhiteSpace();
            Condition.Requires(pageViewModel).IsNotNull();

            _moduleRenderer = moduleRenderer;
            _sectionName = sectionName;
            _pageViewModel = pageViewModel;
            _htmlHelper = htmlHelper;
        }

        #endregion

        #region state properties

        private string _sectionName;
        private string _wrappingTagName = null;
        private bool _allowMultipleModules = false;
        private int? _emptyContentMinHeight = null;
        private Dictionary<string, string> _additonalHtmlAttributes = null;

        #endregion

        #region public chaining methods

        /// <summary>
        /// Allows multiple modules to be added to this section.
        /// </summary>
        /// <returns></returns>
        public IPageTemplateSectionTagBuilder AllowMultipleModules()
        {
            _allowMultipleModules = true;
            return this;
        }

        /// <summary>
        /// Wraps the section with a specific tag type. If set this is wrapped in both edit mode and 
        /// non edit mode. If this isn't set then the content will still be wrapped in a div but only in edit mdoe
        /// and only if there are multiple modules or the child module has more than 1 root element.
        /// </summary>
        /// <param name="tagName">Name of the wrapping tag e.g. div, p, header</param>
        /// <param name="htmlAttributes">Html attributes to apply to the wrapping tag.</param>
        public IPageTemplateSectionTagBuilder WrapWithTag(string tagName, object htmlAttributes = null)
        {
            Condition.Requires(tagName).IsNotNullOrWhiteSpace();

            _wrappingTagName = tagName;

            if (htmlAttributes != null)
            {
                var attributes = new Dictionary<string, string>();

                foreach (var attr in HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes))
                {
                    attributes.Add(attr.Key, Convert.ToString(attr.Value));
                }

                _additonalHtmlAttributes = attributes;
            }

            return this;
        }

        /// <summary>
        /// Sets a minimum height (in pixels) for the section when it contains no modules. Useful to give a
        /// larger roll-over area for the section when it is empty.
        /// </summary>
        public IPageTemplateSectionTagBuilder EmptyContentMinHeight(int minHeight)
        {
            _emptyContentMinHeight = minHeight;
            return this;
        }
        
        #endregion

        #region implementation

        public string ToHtmlString()
        {
            var pageSection = _pageViewModel
                .Page
                .Sections
                .SingleOrDefault(s => s.Name == _sectionName);

            if (pageSection != null)
            {
                return RenderSection(pageSection);
            }
            else 
            {
                var msg = "WARNING: The section '" + _sectionName + "' cannot be found in the database";

                Debug.Assert(pageSection != null, msg);
                if (_pageViewModel.Page.WorkFlowStatus != WorkFlowStatus.Published)
                {
                    return "<!-- " + msg + " -->";
                }
            }

            return string.Empty;
        }

        private string RenderSection(PageSectionRenderDetails pageSection)
        {
            string modulesHtml = string.Empty;

            if (pageSection.Modules.Any())
            {
                var moduleHtmlParts = pageSection
                            .Modules
                            .Select(m => _moduleRenderer.RenderModule(_htmlHelper, _pageViewModel, m));

                if (!_allowMultipleModules)
                {
                    // If for some reason another module has been added in error, make sure we only display one.
                    modulesHtml = moduleHtmlParts.Last();
                }
                else
                {
                    modulesHtml = string.Join(string.Empty, moduleHtmlParts);
                }
            }
            else if (!_allowMultipleModules)
            {
                // If there are no modules and this is a single module section
                // add a placeholder element so we always have a menu
                modulesHtml = _moduleRenderer.RenderPlaceholderModule(_emptyContentMinHeight);
            }

            // If we're not in edit mode just return the modules.
            if (!_pageViewModel.IsPageEditMode)
            {
                if (_wrappingTagName != null)
                {
                    return WrapInTag(modulesHtml);
                }

                return modulesHtml;
            }

            var attrs = new Dictionary<string, string>();
            attrs.Add("data-cms-page-template-section-id", pageSection.PageTemplateSectionId.ToString());
            attrs.Add("data-cms-page-section-name", pageSection.Name);
            attrs.Add("data-cms-page-section", string.Empty);
            attrs.Add("class", "cofoundry__sv-section");
            
            if (_allowMultipleModules)
            {
                attrs.Add("data-cms-multi-module", "true");

                if (_emptyContentMinHeight.HasValue)
                {
                    attrs.Add("style", "min-height:" + _emptyContentMinHeight + "px");
                }
            }

            return WrapInTag(modulesHtml, attrs);
        }

        private string WrapInTag(string modulesHtml, Dictionary<string, string> additionalAttributes = null)
        {
            var html = new HtmlDocument();
            html.LoadHtml(modulesHtml.Trim());

            HtmlNode wrapper;

            // No need to wrap if its a single modle with a single outer node.
            if (!_allowMultipleModules && html.DocumentNode.ChildNodes.Count == 1 && _wrappingTagName == null)
            {
                wrapper = html.DocumentNode.ChildNodes.First();
            }
            else
            {
                var wrap = new HtmlDocument();
                wrapper = wrap.CreateElement(_wrappingTagName ?? DEFAULT_TAG);
                wrapper.InnerHtml = modulesHtml;
            }

            wrapper.MergeAttributes(_additonalHtmlAttributes);

            if (additionalAttributes != null)
            {
                wrapper.MergeAttributes(additionalAttributes);
            }

            return wrapper.OuterHtml;
        }

        #endregion
    }
}
