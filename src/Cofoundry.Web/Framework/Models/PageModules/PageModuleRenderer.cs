using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using HtmlAgilityPack;
using Cofoundry.Domain;
using Cofoundry.Core.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cofoundry.Web
{
    /// <summary>
    /// Controls rendering of page modules, rendering the razor templates out to 
    /// a string
    /// </summary>
    public class PageModuleRenderer : IPageModuleRenderer
    {
        private readonly IPageModuleTypeViewFileLocator _pageModuleTypeViewFileLocator;

        public PageModuleRenderer(
            IPageModuleTypeViewFileLocator pageModuleTypeViewFileLocator
            )
        {
            _pageModuleTypeViewFileLocator = pageModuleTypeViewFileLocator;
        }

        #region public methods

        /// <summary>
        /// Renders a page module by finding the template and applying the specified model to it
        /// </summary>
        /// <param name="controllerContext">ControllerContext is required so we can render the razor view</param>
        /// <param name="pageViewModel">The view model for the page being rendered</param>
        /// <param name="moduleViewModel">The view model for the module being rendered</param>
        /// <returns>The rednered module html</returns>
        public string RenderModule(
            ViewContext viewContext, 
            IEditablePageViewModel pageViewModel, 
            IEntityVersionPageModuleRenderDetails moduleViewModel
            )
        {
            var displayData = GetModuleDisplayData(pageViewModel, moduleViewModel);
            string viewPath = GetViewPath(moduleViewModel);

            var viewRenderer = new RazorViewRenderer();
            string html = viewRenderer.RenderPartialView(viewContext, viewPath, displayData);

            if (pageViewModel.IsPageEditMode)
            {
                html = ParseModuleForEditing(html, moduleViewModel);
            }

            return html;
        }

        /// <summary>
        /// Renders an default placeholder element for use when a module has not yet been added to a section.
        /// </summary>
        /// <param name="minHeight">the min-height to apply to the element.</param>
        public string RenderPlaceholderModule(int? minHeight = null)
        {
            string styles = string.Empty;
            if (minHeight.HasValue)
            {
                styles = " style='min-height:" + minHeight + "px'";
            }
            var html = ParseModuleForEditing("<div class='cofoundry-sv__module-empty'" + styles + "></div>", null);

            return html;
        }

        #endregion

        #region private helpers

        private IPageModuleDisplayModel GetModuleDisplayData(IEditablePageViewModel pageViewModel, IEntityVersionPageModuleRenderDetails moduleViewModel)
        {
            var displayData = moduleViewModel.DisplayModel as IPageModuleWithParentPageData;

            if (displayData != null)
            {
                displayData.ParentPage = pageViewModel;
            }

            return moduleViewModel.DisplayModel;
        }

        private string GetViewPath(IEntityVersionPageModuleRenderDetails moduleViewModel)
        {
            string viewPath;
            string fileName = moduleViewModel.ModuleType.FileName;

            if (moduleViewModel.Template == null)
            {
                viewPath = _pageModuleTypeViewFileLocator.GetPathByFileName(fileName);
            }
            else
            {
                viewPath = _pageModuleTypeViewFileLocator.GetTemplatePathByTemplateFileName(fileName, moduleViewModel.Template.FileName);
            }

            if (viewPath == null) return null;
            return "~" + viewPath;
        }

        /// <summary>
        /// Parses the rendered html string of a module, when in edit mode. It adds attributes used for hovering and interacting
        /// with a module in siteviewer mode. It also adds a css class.
        /// </summary>
        private string ParseModuleForEditing(string moduleHtml, IEntityVersionPageModuleRenderDetails moduleViewModel)
        {
            string entityType = moduleViewModel is CustomEntityVersionPageModuleRenderDetails ? "custom-entity" : "page";

            var attrs = new Dictionary<string, string>();
            attrs.Add("class", "cofoundry-sv__module");
            attrs.Add("data-cms-" + entityType + "-section-module", string.Empty);
            
            if (moduleViewModel != null)
            {
                attrs.Add("data-cms-version-module-id", moduleViewModel.EntityVersionPageModuleId.ToString());
                attrs.Add("data-cms-page-module-type-id", moduleViewModel.ModuleType.PageModuleTypeId.ToString());
                attrs.Add("data-cms-page-module-title", moduleViewModel.ModuleType.Name.ToString());
            }

            var editModuleHtml = new HtmlDocument();
            editModuleHtml.LoadHtml(moduleHtml.Trim());

            var nodes = editModuleHtml.DocumentNode.ChildNodes;

            if (nodes.Count == 1)
            {
                var node = nodes.Single();

                if (node.NodeType == HtmlNodeType.Element)
                {
                    return node 
                        .MergeAttributes(attrs)
                        .OuterHtml;
                }
            }

            var wrap = new HtmlDocument();
            var wrapper = wrap.CreateElement("div");
            wrapper.InnerHtml = moduleHtml;
            wrapper.MergeAttributes(attrs);

            return wrapper.OuterHtml;
        }

        #endregion
    }
}
