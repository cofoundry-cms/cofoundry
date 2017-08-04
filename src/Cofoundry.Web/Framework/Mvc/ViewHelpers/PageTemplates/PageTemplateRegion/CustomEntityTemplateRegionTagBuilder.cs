using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Text.Encodings.Web;
using Cofoundry.Core;

namespace Cofoundry.Web
{
    /// <summary>
    /// Fluent builder for defining a page template custom entity region configuration using method chaining
    /// </summary>
    public class CustomEntityTemplateRegionTagBuilder<TModel> : ICustomEntityTemplateRegionTagBuilder<TModel>
        where TModel : ICustomEntityPageDisplayModel 
    {
        #region constructor

        const string DEFAULT_TAG ="div";
        private readonly ICustomEntityPageViewModel<TModel> _customEntityViewModel;
        private readonly ViewContext _viewContext;
        private readonly IPageBlockRenderer _blockRenderer;
        private readonly IPageBlockTypeDataModelTypeFactory _pageBlockDataModelTypeFactory;
        private readonly IPageBlockTypeFileNameFormatter _blockTypeFileNameFormatter;
        private readonly List<string> _allowedBlocks = new List<string>();

        public CustomEntityTemplateRegionTagBuilder(
            IPageBlockRenderer blockRenderer,
            IPageBlockTypeDataModelTypeFactory pageBlockDataModelTypeFactory,
            IPageBlockTypeFileNameFormatter pageBlockTypeFileNameFormatter,
            ViewContext viewContext,
            ICustomEntityPageViewModel<TModel> customEntityViewModel, 
            string regionName
            )
        {
            if (regionName == null) throw new ArgumentNullException(nameof(regionName));
            if (string.IsNullOrWhiteSpace(regionName)) throw new ArgumentEmptyException(nameof(regionName));
            if (customEntityViewModel == null) throw new ArgumentNullException(nameof(customEntityViewModel));

            _blockRenderer = blockRenderer;
            _pageBlockDataModelTypeFactory = pageBlockDataModelTypeFactory;
            _blockTypeFileNameFormatter = pageBlockTypeFileNameFormatter;
            _regionName = regionName;
            _customEntityViewModel = customEntityViewModel;
            _viewContext = viewContext;
        }

        #endregion

        #region state properties

        private string _output = null;
        private string _regionName;
        private string _wrappingTagName = null;
        private bool _allowMultipleBlocks = false;
        private int? _emptyContentMinHeight = null;
        private Dictionary<string, string> _additonalHtmlAttributes = null;
        private Dictionary<string, object> _permittedBlockTypes = new Dictionary<string, object>();

        #endregion

        #region public chaining methods

        /// <summary>
        /// By default only a single block is allowed per region. This method configures the
        /// region to allow an unlimited number of blocks to be added
        /// </summary>
        /// <returns>ICustomEntityTemplateRegionTagBuilder for method chaining</returns>
        public ICustomEntityTemplateRegionTagBuilder<TModel> AllowMultipleBlocks()
        {
            _allowMultipleBlocks = true;
            return this;
        }

        /// <summary>
        /// Wraps blocks within the region in a specific html tag. Each region
        /// needs to have a single container node in page edit mode and by default a div
        /// element is used if a single container cannot be found in the rendered output.
        /// Use this setting to control this tag. If a tag is specified here the output
        /// will always be wrapped irrespecitive of whether the page is in edit mode to maintain
        /// consistency.
        /// </summary>
        /// <param name="tagName">Name of the element to wrap the output in e.g. div, p, header</param>
        /// <param name="htmlAttributes">Html attributes to apply to the wrapping tag.</param>
        /// <returns>ICustomEntityTemplateRegionTagBuilder for method chaining</returns>
        public ICustomEntityTemplateRegionTagBuilder<TModel> WrapWithTag(string tagName, object htmlAttributes = null)
        {
            if (tagName == null) throw new ArgumentNullException(nameof(tagName));
            if (string.IsNullOrWhiteSpace(tagName)) throw new ArgumentEmptyException(nameof(tagName));

            _wrappingTagName = tagName;
            _additonalHtmlAttributes = TemplateRegionTagBuilderHelper.ParseHtmlAttributesFromAnonymousObject(htmlAttributes);

            return this;
        }

        /// <summary>
        /// Sets the default height of the region when it contains no
        /// blocks and the page is in edit mode. This makes it easier to visualize
        /// and select the region when a template is first created.
        /// </summary>
        /// <param name="minHeight">The minimum height in pixels</param>
        /// <returns>ICustomEntityTemplateRegionTagBuilder for method chaining</returns>
        public ICustomEntityTemplateRegionTagBuilder<TModel> EmptyContentMinHeight(int minHeight)
        {
            _emptyContentMinHeight = minHeight;
            return this;
        }
        
        /// <summary>
        /// Permits the block type associated with the specified data model type
        /// to be selected and used in this page region. By default all block type are 
        /// available but using this method changes the block type selection to an 'opt-in'
        /// configuration.
        /// </summary>
        /// <typeparam name="TBlockType">The data model type of the block type to register</typeparam>
        /// <returns>ICustomEntityTemplateRegionTagBuilder for method chaining</returns>
        public ICustomEntityTemplateRegionTagBuilder<TModel> AllowBlockType<TBlockType>() where TBlockType : IPageBlockTypeDataModel
        {
            AddBlockToAllowedTypes(typeof(TBlockType).Name);
            return this;
        }

        /// <summary>
        /// Permits the specified block type to be selected and used in this page 
        /// region. By default all block type are available but using this method changes 
        /// the block type selection to an 'opt-in' configuration.
        /// </summary>
        /// <param name="blockTypeName">The name of the block type to register e.g. "RawHtml" or "PlainText"</param>
        /// <returns>ICustomEntityTemplateRegionTagBuilder for method chaining</returns>
        public ICustomEntityTemplateRegionTagBuilder<TModel> AllowBlockType(string blockTypeName)
        {
            AddBlockToAllowedTypes(blockTypeName);
            return this;
        }

        /// <summary>
        /// Permits the specified block types to be selected and used in this page 
        /// region. By default all block type are available but using this method changes 
        /// the block type selection to an 'opt-in' configuration.
        /// configuration.
        /// </summary>
        /// <param name="blockTypeNames">The names of the block types to register e.g. "RawHtml" or "PlainText"</param>
        /// <returns>ICustomEntityTemplateRegionTagBuilder for method chaining</returns>
        public ICustomEntityTemplateRegionTagBuilder<TModel> AllowBlockTypes(params string[] blockTypeNames)
        {
            foreach (var blockTypeName in blockTypeNames)
            {
                AddBlockToAllowedTypes(blockTypeName);
            }
            return this;
        }

        private void AddBlockToAllowedTypes(string blockTypeName)
        {
            // Get the file name if we haven't already got it, e.g. we could have the file name or 
            // full type name passed into here (we shouldn't be fussy)
            var fileName = _blockTypeFileNameFormatter.FormatFromDataModelName(blockTypeName);

            // Validate the model type, will throw exception if not implemented
            var blockType = _pageBlockDataModelTypeFactory.CreateByPageBlockTypeFileName(fileName);

            // Make sure we have the correct name casing
            var formattedBlockTypeName = _blockTypeFileNameFormatter.FormatFromDataModelType(blockType);

            _permittedBlockTypes.Add(formattedBlockTypeName, null);
        }

        #endregion

        #region rendering

        /// <summary>
        /// This method must be called at the end of the region definition to build and render the
        /// region.
        /// </summary>
        public async Task<ICustomEntityTemplateRegionTagBuilder<TModel>> InvokeAsync()
        {
            var region = _customEntityViewModel
                .CustomEntity
                .Regions
                .SingleOrDefault(s => s.Name == _regionName);

            if (region != null)
            {
                _output = await RenderRegion(region);
            }
            else
            {
                var msg = "WARNING: The custom entity region '" + _regionName + "' cannot be found in the database";

                Debug.Assert(region != null, msg);
                if (_customEntityViewModel.CustomEntity.WorkFlowStatus != WorkFlowStatus.Published)
                {
                    _output = "<!-- " + msg + " -->";
                }
            }

            return this;
        }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            Debug.Assert(_output != null, $"Template region '{ _regionName }' definition does not call { nameof(InvokeAsync)}().");

            if (_output != null)
            {
                writer.Write(_output);
            }
        }

        private async Task<string> RenderRegion(CustomEntityPageRegionRenderDetails pageRegion)
        {
            string blocksHtml = string.Empty;

            if (pageRegion.Blocks.Any())
            {
                var renderingTasks = pageRegion
                        .Blocks
                        .Select(m => _blockRenderer.RenderBlockAsync(_viewContext, _customEntityViewModel, m));

                var blockHtmlParts = await Task.WhenAll(renderingTasks);

                if (!_allowMultipleBlocks)
                {
                    // If for some reason another block has been added in error, make sure we only display one.
                    blocksHtml = blockHtmlParts.Last();
                }
                else
                {
                    blocksHtml = string.Join(string.Empty, blockHtmlParts);
                }
            }
            else if (!_allowMultipleBlocks)
            {
                // If there are no blocks and this is a single block region
                // add a placeholder element so we always have a menu
                blocksHtml = _blockRenderer.RenderPlaceholderBlock(_emptyContentMinHeight);
            }

            // If we're not in edit mode just return the blocks.
            if (!_customEntityViewModel.IsPageEditMode) // TODO: IsCustom entity mode
            {
                if (_wrappingTagName != null)
                {
                    return TemplateRegionTagBuilderHelper.WrapInTag(
                        blocksHtml,
                        _wrappingTagName,
                        _allowMultipleBlocks,
                        _additonalHtmlAttributes
                        );
                }

                return blocksHtml;
            }

            var attrs = new Dictionary<string, string>();
            attrs.Add("data-cms-page-template-region-id", pageRegion.PageTemplateRegionId.ToString());
            attrs.Add("data-cms-page-region-name", pageRegion.Name);
            attrs.Add("data-cms-custom-entity-region", string.Empty);

            attrs.Add("class", "cofoundry__sv-region");

            if (_permittedBlockTypes.Any())
            {
                var permittedBlockTypes = _permittedBlockTypes.Select(m => m.Key);
                attrs.Add("data-cms-page-region-permitted-block-types", string.Join(",", permittedBlockTypes));
            }

            if (_allowMultipleBlocks)
            {
                attrs.Add("data-cms-multi-block", "true");

                if (_emptyContentMinHeight.HasValue)
                {
                    attrs.Add("style", "min-height:" + _emptyContentMinHeight + "px");
                }
            }
            
            return TemplateRegionTagBuilderHelper.WrapInTag(
                blocksHtml,
                _wrappingTagName,
                _allowMultipleBlocks,
                _additonalHtmlAttributes,
                attrs
                );
        }

        #endregion
    }
}
