using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Cofoundry.Web
{
    public static class BreadcrumbUIControlExtension
    {
        #region public methods

        /// <summary>
        /// Renders a list of breadcrumb links using the default template.
        /// </summary>
        public static HtmlString Breadcrumbs(this IUIViewHelper helper)
        {
            var vm = GetBreadCrumbsViewModel(helper);
            if (vm == null) return null;

            var list = new TagBuilder("ol");
            list.Attributes["id"] = "breadcrumbs";
            list.Attributes["role"] = "navigation";

            foreach (BreadcrumbViewModel breadCrumb in vm.Breadcrumbs)
            {
                var itemEl = new TagBuilder("li");

                if (breadCrumb == vm.Breadcrumbs.First())
                {
                    itemEl.AddCssClass("first");
                }

                if (breadCrumb == vm.Breadcrumbs.Last())
                {
                    itemEl.AddCssClass("last");
                }

                if (breadCrumb.HasHref)
                {
                    var link = new TagBuilder("a");
                    link.Attributes.Add("href", breadCrumb.Href);
                    link.SetInnerText(breadCrumb.Title);
                    itemEl.InnerHtml = link.ToString();
                }
                else
                {
                    itemEl.SetInnerText(breadCrumb.Title);
                }

                list.InnerHtml += itemEl.ToString();
            }

            return new HtmlString(list.ToString());
        }

        /// <summary>
        /// Renders a list of breadcrumb links using the specified template.
        /// </summary>
        /// <param name="partialViewName">The name of the partialview to use when rendering the breadcrumb view model</param>
        public static HtmlString Breadcrumbs(this IUIViewHelper helper, string partialViewName)
        {
            return helper.HtmlHelper.Partial(partialViewName, GetBreadCrumbsViewModel(helper));
        }

        #endregion

        #region private helpers

        private static BreadcrumbsViewModel GetBreadCrumbsViewModel(IUIViewHelper helper)
        {
            var pageViewModel = helper.HtmlHelper.ViewData.Model as IPageRoutableViewModel;

            if (pageViewModel != null)
            {
                return GetBreadcrumbs(pageViewModel.PageRoutingHelper);
            }

            return null;
        }

        private static BreadcrumbsViewModel GetBreadcrumbs(PageRoutingHelper pageRouting)
        {
            if (pageRouting.CurrentPageRoute == null) return null;
            var version = pageRouting.CurrentPageRoute.Versions.GetVersionRouting(pageRouting.VisualEditorMode.ToWorkFlowStatusQuery(), pageRouting.CurrentPageVersionId);
            var crumbs = new List<BreadcrumbViewModel>
                {
                    new BreadcrumbViewModel { Title = version.Title, Href = string.Empty }
                };

            var currentDirectory = pageRouting.CurrentPageRoute.WebDirectory;

            if (pageRouting.CurrentPageRoute.IsDirectoryDefaultPage())
            {
                currentDirectory = GetParentDirectory(pageRouting, currentDirectory);
            }

            while (currentDirectory != null)
            {
                var parentDirectory = GetParentDirectory(pageRouting, currentDirectory);
                var title = parentDirectory.ParentWebDirectoryId.HasValue ? currentDirectory.Name : "Home";
                crumbs.Insert(0, new BreadcrumbViewModel { Title = title, Href = currentDirectory.FullUrlPath });

                currentDirectory = parentDirectory;
            }

            return new BreadcrumbsViewModel { Breadcrumbs = crumbs };
        }

        private static WebDirectoryRoute GetParentDirectory(PageRoutingHelper pageRouting, WebDirectoryRoute currentDirectory)
        {
            return pageRouting.WebDirectoryRoutes.SingleOrDefault(d => d.WebDirectoryId == currentDirectory.ParentWebDirectoryId);
        }

        #endregion
    }
}