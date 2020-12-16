using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Helper for mapping page and custom entity version page block data from an 
    /// unstructured db source to a display model instance.
    /// </summary>
    public class PageVersionBlockModelMapper : IPageVersionBlockModelMapper
    {
        private static readonly MethodInfo _mapGenericMethod = typeof(PageVersionBlockModelMapper).GetMethod(nameof(MapGeneric), BindingFlags.NonPublic | BindingFlags.Instance);

        #region constructor

        private readonly IServiceProvider _serviceProvider;
        private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
        private readonly IPageBlockTypeDataModelTypeFactory _pageBlockDataModelTypeFactory;

        public PageVersionBlockModelMapper(
            IServiceProvider serviceProvider,
            IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
            IPageBlockTypeDataModelTypeFactory pageBlockDataModelTypeFactory
            )
        {
            _serviceProvider = serviceProvider;
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
        /// <param name="entityBlocks">The version data to get the serialized model from.</param>
        /// <param name="publishStatus">
        /// The publish status of the parent page or custom entity 
        /// being mapped. This is provided so dependent entities can use
        /// the same publish status.
        /// </param>
        /// <param name="executionContext">
        /// The execution context from the caller which can be used in
        /// any child queries to ensure any elevated permissions are 
        /// passed down the chain of execution.
        /// </param>
        /// <returns>
        /// Dictionary of mapped display models, with a key (block version id) that can be 
        /// used to identify them.
        /// </returns>
        public virtual async Task<IReadOnlyDictionary<int, IPageBlockTypeDisplayModel>> MapDisplayModelAsync(
            string typeName, 
            IEnumerable<IEntityVersionPageBlock> entityBlocks, 
            PublishStatusQuery publishStatus,
            IExecutionContext executionContext
            )
        {
            // Find the data-provider class for this block type
            Type modelType = _pageBlockDataModelTypeFactory.CreateByPageBlockTypeFileName(typeName);
            
            if (typeof(IPageBlockTypeDisplayModel).IsAssignableFrom(modelType))
            {
                // We can serialize directly to the display model
                var displayModels = new Dictionary<int, IPageBlockTypeDisplayModel>();
                foreach (var pageBlock in entityBlocks)
                {
                    var displayModel = (IPageBlockTypeDisplayModel)_dbUnstructuredDataSerializer.Deserialize(pageBlock.SerializedData, modelType);
                    var versionBlockId = pageBlock.GetVersionBlockId();

                    if (displayModels.ContainsKey(versionBlockId))
                    {
                        throw new Exception($"A block with a version id of {versionBlockId} has already been added to the mapping collection.");
                    }

                    displayModels.Add(versionBlockId, displayModel);
                }

                return displayModels;
            }
            else
            {
                var blockWorkflowStatus = publishStatus.ToRelatedEntityQueryStatus();

                // We have to use a mapping class to do some custom mapping
                var displayModels = (Task<IReadOnlyDictionary<int, IPageBlockTypeDisplayModel>>)_mapGenericMethod
                    .MakeGenericMethod(modelType)
                    .Invoke(this, new object[] { entityBlocks, blockWorkflowStatus, executionContext });

                return await displayModels;
            }
        }

        /// <summary>
        /// Maps a single page block data model to a concrete
        /// display model.
        /// </summary>
        /// <param name="typeName">The block type name e.g. 'PlainText', 'RawHtml'.</param>
        /// <param name="pageBlock">The version data to get the serialized model from.</param>
        /// <param name="publishStatus">
        /// The publish status of the parent page or custom entity 
        /// being mapped. This is provided so dependent entities can use
        /// the same publish status.
        /// </param>
        /// <param name="executionContext">
        /// The execution context from the caller which can be used in
        /// any child queries to ensure any elevated permissions are 
        /// passed down the chain of execution.
        /// </param>
        /// <returns>Mapped display model.</returns>
        public virtual async Task<IPageBlockTypeDisplayModel> MapDisplayModelAsync(
            string typeName,
            IEntityVersionPageBlock pageBlock, 
            PublishStatusQuery publishStatus,
            IExecutionContext executionContext
            )
        {
            var mapped = await MapDisplayModelAsync(
                typeName, 
                new IEntityVersionPageBlock[] { pageBlock }, 
                publishStatus,
                executionContext
                );

            var id = pageBlock.GetVersionBlockId();
            if (mapped.ContainsKey(id)) return mapped[id];

            return null;
        }

        /// <summary>
        /// Deserialized a page block data model to a stongly typed model.
        /// </summary>
        /// <param name="typeName">The block type name e.g. 'PlainText', 'RawHtml'.</param>
        /// <param name="pageBlock">The version data to get the serialized model from.</param>
        /// <returns>Strongly typed data model including deserialized data.</returns>
        public virtual IPageBlockTypeDataModel MapDataModel(string typeName, IEntityVersionPageBlock pageBlock)
        {
            Type modelType = _pageBlockDataModelTypeFactory.CreateByPageBlockTypeFileName(typeName);
            var model = (IPageBlockTypeDataModel)_dbUnstructuredDataSerializer.Deserialize(pageBlock.SerializedData, modelType);

            return model;
        }

        #endregion

        #region privates
        
        protected async Task<IReadOnlyDictionary<int, IPageBlockTypeDisplayModel>> MapGeneric<TDataModel>(
            IEnumerable<IEntityVersionPageBlock> pageBlocks, 
            PublishStatusQuery publishStatusQuery,
            IExecutionContext executionContext
            ) where TDataModel : IPageBlockTypeDataModel
        {
            var mapperType = typeof(IPageBlockTypeDisplayModelMapper<TDataModel>);
            var mapper = (IPageBlockTypeDisplayModelMapper<TDataModel>)_serviceProvider.GetService(mapperType);
            if (mapper == null)
            {
                string msg = @"{0} does not implement IPageBlockDisplayModel and no custom mapper could be found. You must create 
                               a class that implements IPageBlockDisplayModelMapper<{0}> if you are using a custom display model. Full type name: {1}";
                throw new Exception(string.Format(msg, typeof(TDataModel).Name, typeof(TDataModel).FullName));
            }

            var dataModels = new List<PageBlockTypeDisplayModelMapperInput<TDataModel>>();

            foreach (var pageBlock in pageBlocks)
            {
                var mapperModel = new PageBlockTypeDisplayModelMapperInput<TDataModel>();
                mapperModel.DataModel = (TDataModel)_dbUnstructuredDataSerializer.Deserialize(pageBlock.SerializedData, typeof(TDataModel));
                mapperModel.VersionBlockId = pageBlock.GetVersionBlockId();
                dataModels.Add(mapperModel);
            }

            var context = new PageBlockTypeDisplayModelMapperContext<TDataModel>(dataModels, publishStatusQuery, executionContext);
            var result = new PageBlockTypeDisplayModelMapperResult<TDataModel>(dataModels.Count);
            await mapper.MapAsync(context, result);

            return result.Items;
        }

        #endregion
    }
}
