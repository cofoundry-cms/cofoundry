using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Cofoundry.Core.Web;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cofoundry.Web;

/// <inheritdoc/>
public class PageBlockRenderer : IPageBlockRenderer
{
    private readonly IPageBlockTypeViewFileLocator _pageBlockTypeViewFileLocator;
    private readonly IRazorViewRenderer _razorViewRenderer;

    public PageBlockRenderer(
        IPageBlockTypeViewFileLocator pageBlockTypeViewFileLocator,
        IRazorViewRenderer razorViewRenderer
        )
    {
        _pageBlockTypeViewFileLocator = pageBlockTypeViewFileLocator;
        _razorViewRenderer = razorViewRenderer;
    }

    public async Task<string> RenderBlockAsync(
        ViewContext viewContext,
        IEditablePageViewModel pageViewModel,
        IEntityVersionPageBlockRenderDetails blockViewModel
        )
    {
        var displayData = GetBlockDisplayData(pageViewModel, blockViewModel);
        string viewPath = GetViewPath(blockViewModel);

        string html = await _razorViewRenderer.RenderViewAsync(viewContext, viewPath, displayData);

        if (pageViewModel.IsPageEditMode)
        {
            html = ParseBlockHtmlForEditing(html, blockViewModel);
        }

        return html;
    }

    public string RenderPlaceholderBlock(int? minHeight = null)
    {
        string styles = string.Empty;
        if (minHeight.HasValue)
        {
            styles = " style='min-height:" + minHeight + "px'";
        }
        var html = ParseBlockHtmlForEditing("<div class='cofoundry-sv__block-empty'" + styles + "></div>", null);

        return html;
    }

    private IPageBlockTypeDisplayModel GetBlockDisplayData(IEditablePageViewModel pageViewModel, IEntityVersionPageBlockRenderDetails blockViewModel)
    {
        var displayData = blockViewModel.DisplayModel as IPageBlockWithParentPageData;

        if (displayData != null)
        {
            displayData.ParentPage = pageViewModel;
        }

        return blockViewModel.DisplayModel;
    }

    private string GetViewPath(IEntityVersionPageBlockRenderDetails blockViewModel)
    {
        string viewPath;
        string fileName = blockViewModel.BlockType.FileName;

        if (blockViewModel.Template == null)
        {
            viewPath = _pageBlockTypeViewFileLocator.GetPathByFileName(fileName);
        }
        else
        {
            viewPath = _pageBlockTypeViewFileLocator.GetTemplatePathByTemplateFileName(fileName, blockViewModel.Template.FileName);
        }

        if (viewPath == null) return null;
        return "~" + viewPath;
    }

    /// <summary>
    /// Parses the rendered html string of a block, when in edit mode. It adds attributes used for hovering and interacting
    /// with a block in siteviewer mode. It also adds a css class.
    /// </summary>
    private string ParseBlockHtmlForEditing(string blockHtml, IEntityVersionPageBlockRenderDetails blockViewModel)
    {
        string entityType = blockViewModel is CustomEntityVersionPageBlockRenderDetails ? "custom-entity" : "page";

        var attrs = new Dictionary<string, string>();
        attrs.Add("class", "cofoundry-sv__block");
        attrs.Add("data-cms-" + entityType + "-region-block", string.Empty);

        if (blockViewModel != null)
        {
            attrs.Add("data-cms-version-block-id", blockViewModel.EntityVersionPageBlockId.ToString());
            attrs.Add("data-cms-page-block-type-id", blockViewModel.BlockType.PageBlockTypeId.ToString());
            attrs.Add("data-cms-page-block-title", blockViewModel.BlockType.Name.ToString());
        }

        var parser = new HtmlParser();
        var document = parser.ParseDocument(blockHtml.Trim());

        var elements = document.Body.Children;

        if (elements.Length == 1)
        {
            var element = elements.Single();

            if (element.NodeType == NodeType.Element)
            {
                AngleSharpHelper.MergeAttributes(element, attrs);

                return element.OuterHtml;
            }
        }

        var wrapper = document.CreateElement("div");
        AngleSharpHelper.WrapChildren(document.Body, wrapper);
        AngleSharpHelper.MergeAttributes(wrapper, attrs);

        return wrapper.OuterHtml;
    }
}