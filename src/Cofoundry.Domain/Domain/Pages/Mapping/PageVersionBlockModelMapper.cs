using Cofoundry.Core.Reflection.Internal;
using Cofoundry.Domain.Data;
using System.Reflection;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Default implementation of <see cref="IPageRenderSummaryMapper"/>.
/// </summary>
public class PageVersionBlockModelMapper : IPageVersionBlockModelMapper
{
    private static readonly MethodInfo _mapGenericMethod = MethodReferenceHelper.GetPrivateInstanceMethod<PageVersionBlockModelMapper>(nameof(MapGeneric));

    private readonly IServiceProvider _serviceProvider;
    private readonly IDbUnstructuredDataSerializer _dbUnstructuredDataSerializer;
    private readonly IPageBlockTypeDataModelTypeFactory _pageBlockDataModelTypeFactory;
    private readonly IEmptyDataModelFactory _emptyDataModelFactory;

    public PageVersionBlockModelMapper(
        IServiceProvider serviceProvider,
        IDbUnstructuredDataSerializer dbUnstructuredDataSerializer,
        IPageBlockTypeDataModelTypeFactory pageBlockDataModelTypeFactory,
        IEmptyDataModelFactory emptyDataModelFactory
        )
    {
        _serviceProvider = serviceProvider;
        _dbUnstructuredDataSerializer = dbUnstructuredDataSerializer;
        _pageBlockDataModelTypeFactory = pageBlockDataModelTypeFactory;
        _emptyDataModelFactory = emptyDataModelFactory;
    }

    /// <inheritdoc/>
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
                var displayModel = _dbUnstructuredDataSerializer.Deserialize(pageBlock.SerializedData, modelType) as IPageBlockTypeDisplayModel;
                if (displayModel == null)
                {
                    // if there's a problem deserializing, then skip. Error should be logged by the serializer
                    continue;
                }

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
            var mapGenericResult = _mapGenericMethod
                .MakeGenericMethod(modelType)
                .Invoke(this, new object[] { entityBlocks, blockWorkflowStatus, executionContext }) as Task<IReadOnlyDictionary<int, IPageBlockTypeDisplayModel>>;

            if (mapGenericResult == null)
            {
                throw new InvalidCastException($"{nameof(_mapGenericMethod)} did not return the expected result type.");
            }

            return await mapGenericResult;
        }
    }

    /// <inheritdoc/>
    public virtual async Task<IPageBlockTypeDisplayModel?> MapDisplayModelAsync(
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
        if (mapped.ContainsKey(id))
        {
            return mapped[id];
        }

        return null;
    }

    /// <inheritdoc/>
    public virtual IPageBlockTypeDataModel MapDataModel(string typeName, IEntityVersionPageBlock pageBlock)
    {
        Type modelType = _pageBlockDataModelTypeFactory.CreateByPageBlockTypeFileName(typeName);
        var model = _dbUnstructuredDataSerializer.Deserialize(pageBlock.SerializedData, modelType) as IPageBlockTypeDataModel;

        if (model == null)
        {
            model = _emptyDataModelFactory.Create<IPageBlockTypeDataModel>(modelType);
        }

        return model;
    }

    protected async Task<IReadOnlyDictionary<int, IPageBlockTypeDisplayModel>> MapGeneric<TDataModel>(
        IEnumerable<IEntityVersionPageBlock> pageBlocks,
        PublishStatusQuery publishStatusQuery,
        IExecutionContext executionContext
        ) where TDataModel : class, IPageBlockTypeDataModel
    {
        var mapperType = typeof(IPageBlockTypeDisplayModelMapper<TDataModel>);
        var mapper = (IPageBlockTypeDisplayModelMapper<TDataModel>?)_serviceProvider.GetService(mapperType);
        if (mapper == null)
        {
            string msg = $"{typeof(TDataModel).Name} does not implement IPageBlockDisplayModel and no custom mapper could be found. You must create a class that implements IPageBlockDisplayModelMapper<{typeof(TDataModel).Name}> if you are using a custom display model. Full type name: {typeof(TDataModel).FullName}";
            throw new Exception(msg);
        }

        var dataModels = new List<PageBlockTypeDisplayModelMapperInput<TDataModel>>();

        foreach (var pageBlock in pageBlocks)
        {
            var dataModel = _dbUnstructuredDataSerializer.Deserialize<TDataModel>(pageBlock.SerializedData);
            if (dataModel == null)
            {
                dataModel = _emptyDataModelFactory.Create<TDataModel>();
            }
            var mapperModel = new PageBlockTypeDisplayModelMapperInput<TDataModel>
            {
                DataModel = dataModel,
                VersionBlockId = pageBlock.GetVersionBlockId()
            };
            dataModels.Add(mapperModel);
        }

        var context = new PageBlockTypeDisplayModelMapperContext<TDataModel>(dataModels, publishStatusQuery, executionContext);
        var result = new PageBlockTypeDisplayModelMapperResult<TDataModel>(dataModels.Count);
        await mapper.MapAsync(context, result);

        return result.Items;
    }
}