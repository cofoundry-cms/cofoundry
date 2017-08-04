using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Core.DependencyInjection;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Helper for mapping page and custom entity version page block data from an 
    /// unstructured db source to a display model instance.
    /// </summary>
    public class PageVersionBlockModelMapper : IPageVersionBlockModelMapper
    {
        private static readonly MethodInfo _mapGenericMethod = typeof(PageVersionBlockModelMapper).GetMethod(nameof(MapGeneric), BindingFlags.NonPublic | BindingFlags.Instance);

        #region constructor

        private readonly IResolutionContext _resolutionContext;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
        private readonly IPageBlockTypeDataModelTypeFactory _pageBlockDataModelTypeFactory;

        public PageVersionBlockModelMapper(
            IResolutionContext resolutionContext,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            IPageBlockTypeDataModelTypeFactory pageBlockDataModelTypeFactory
            )
        {
            _resolutionContext = resolutionContext;
            _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
            _pageBlockDataModelTypeFactory = pageBlockDataModelTypeFactory;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Maps a batch of the same type of page block data to a collection
        /// of display models ready for rendering.
        /// </summary>
        /// <param name="typeName">The block type name e.g. 'PlainText', 'RawHtml'.</param>
        /// <param name="pageBlocks">The version data to get the serialized model from.</param>
        /// <param name="workflowStatus">
        /// The workflow status of the parent page or custom entity 
        /// being mapped. This is provided so dependent entities can use
        /// the same workflow status.
        /// </param>
        /// <returns>
        /// Collection of mapped display models, wrapped in an output class that
        /// can be used to identify them.
        /// </returns>
        public async Task<List<PageBlockDisplayModelMapperOutput>> MapDisplayModelAsync(
            string typeName, 
            IEnumerable<IEntityVersionPageBlock> pageBlocks, 
            WorkFlowStatusQuery workflowStatus
            )
        {
            // Find the data-provider class for this block type
            Type modelType = _pageBlockDataModelTypeFactory.CreateByPageBlockTypeFileName(typeName);
            
            if (typeof(IPageBlockTypeDisplayModel).IsAssignableFrom(modelType))
            {
                // We can serialize directly to the display model
                var displayModels = new List<PageBlockDisplayModelMapperOutput>();
                foreach (var pageBlock in pageBlocks)
                {
                    var mapperModel = new PageBlockDisplayModelMapperOutput();
                    mapperModel.DisplayModel = (IPageBlockTypeDisplayModel)_dbUnstructuredDataSerializer.Deserialize(pageBlock.SerializedData, modelType);
                    mapperModel.VersionBlockId = pageBlock.GetVersionBlockId();
                    displayModels.Add(mapperModel);
                }

                return displayModels;
            }
            else
            {
                var blockWorkflowStatus = TranslateWorkFlowStatusForBlocks(workflowStatus);

                // We have to use a mapping class to do some custom mapping
                var displayModels = (Task<List<PageBlockDisplayModelMapperOutput>>)_mapGenericMethod
                    .MakeGenericMethod(modelType)
                    .Invoke(this, new object[] { pageBlocks, blockWorkflowStatus });

                return await displayModels;
            }
        }

        /// <summary>
        /// Maps a single page block data model to a concrete
        /// display model.
        /// </summary>
        /// <param name="typeName">The block type name e.g. 'PlainText', 'RawHtml'.</param>
        /// <param name="pageBlock">The version data to get the serialized model from.</param>
        /// <param name="workflowStatus">
        /// The workflow status of the parent page or custom entity 
        /// being mapped. This is provided so dependent entities can use
        /// the same workflow status.
        /// </param>
        /// <returns>Mapped display model.</returns>
        public async Task<IPageBlockTypeDisplayModel> MapDisplayModelAsync(
            string typeName,
            IEntityVersionPageBlock pageBlock, 
            WorkFlowStatusQuery workflowStatus
            )
        {
            var mapped = await MapDisplayModelAsync(typeName, new IEntityVersionPageBlock[] { pageBlock }, workflowStatus);
            return mapped
                .Select(m => m.DisplayModel)
                .SingleOrDefault();
        }

        /// <summary>
        /// Deserialized a page block data model to a stongly typed model.
        /// </summary>
        /// <param name="typeName">The block type name e.g. 'PlainText', 'RawHtml'.</param>
        /// <param name="pageBlock">The version data to get the serialized model from.</param>
        /// <returns>Strongly typed data model including deserialized data.</returns>
        public IPageBlockTypeDataModel MapDataModel(string typeName, IEntityVersionPageBlock pageBlock)
        {
            Type modelType = _pageBlockDataModelTypeFactory.CreateByPageBlockTypeFileName(typeName);
            var model = (IPageBlockTypeDataModel)_dbUnstructuredDataSerializer.Deserialize(pageBlock.SerializedData, modelType);

            return model;
        }

        #endregion

        #region privates

        /// <summary>
        /// When working with child entities, the WorkFlowStatus we apply to
        /// them is not neccessarily the status used to query the parent. If we are 
        /// loading a page using the Draft status, then we cannot expect that all 
        /// dependencies should have a draft version, so we re-write it to Latest.
        /// The same applies if we're loading a specific version.
        /// </summary>
        /// <param name="workflowStatus">The original workflow status of the parent entity.</param>
        private WorkFlowStatusQuery TranslateWorkFlowStatusForBlocks(WorkFlowStatusQuery workflowStatus)
        {
            if (workflowStatus == WorkFlowStatusQuery.Draft || workflowStatus == WorkFlowStatusQuery.SpecificVersion)
            {
                workflowStatus = WorkFlowStatusQuery.Latest;
            }

            return workflowStatus;
        }

        private async Task<List<PageBlockDisplayModelMapperOutput>> MapGeneric<T>(
            IEnumerable<IEntityVersionPageBlock> pageBlocks, 
            WorkFlowStatusQuery workflowStatus
            ) where T : IPageBlockTypeDataModel
        {
            var mapperType = typeof(IPageBlockDisplayModelMapper<T>);
            if (!_resolutionContext.IsRegistered(mapperType))
            {
                string msg = @"{0} does not implement IPageBlockDisplayModel and no custom mapper could be found. You must create 
                               a class that implements IPageBlockDisplayModelMapper<{0}> if you are using a custom display model. Full type name: {1}";
                throw new Exception(string.Format(msg, typeof(T).Name, typeof(T).FullName));
            }

            var mapper = (IPageBlockDisplayModelMapper<T>)_resolutionContext.Resolve(typeof(IPageBlockDisplayModelMapper<T>));
            var dataModels = new List<PageBlockDisplayModelMapperInput<T>>();

            foreach (var pageBlock in pageBlocks)
            {
                var mapperModel = new PageBlockDisplayModelMapperInput<T>();
                mapperModel.DataModel = (T)_dbUnstructuredDataSerializer.Deserialize(pageBlock.SerializedData, typeof(T));
                mapperModel.VersionBlockId = pageBlock.GetVersionBlockId();
                dataModels.Add(mapperModel);
            }

            var results = await mapper.MapAsync(dataModels, workflowStatus);

            return results.ToList();
        }

        #endregion
    }
}
