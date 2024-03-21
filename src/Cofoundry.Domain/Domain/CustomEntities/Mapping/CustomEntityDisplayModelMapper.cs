﻿using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Mapper for mapping typed display models for custom entities.
/// </summary>
public class CustomEntityDisplayModelMapper : ICustomEntityDisplayModelMapper
{
    private readonly IServiceProvider _serviceProvider;
    private const string MapDisplayModelMethodName = nameof(ICustomEntityDisplayModelMapper<ICustomEntityDataModel, ICustomEntityDisplayModel<ICustomEntityDataModel>>.MapDisplayModelAsync);

    public CustomEntityDisplayModelMapper(
        IServiceProvider serviceProvider
        )
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Maps custom entity render data into a specific custom entity
    /// display model type, which can be used to render out view data.
    /// </summary>
    /// <typeparam name="TDisplayModel">Display model type to map to.</typeparam>
    /// <param name="renderDetails">The custom entity model to map, including the raw data model.</param>
    /// <param name="publishStatusQuery">
    /// The query type that should be used to query dependent entities. E.g. if the custom
    /// entity was queried with the Published status query, then any dependent entities should
    /// also be queried as Published.
    /// </param>
    /// <returns>Mapped display model instance.</returns>
    public async Task<TDisplayModel> MapDisplayModelAsync<TDisplayModel>(
        CustomEntityRenderDetails renderDetails,
        PublishStatusQuery publishStatusQuery
        )
        where TDisplayModel : ICustomEntityDisplayModel
    {
        var mapperType = typeof(ICustomEntityDisplayModelMapper<,>).MakeGenericType(renderDetails.Model.GetType(), typeof(TDisplayModel));
        var mapper = _serviceProvider.GetRequiredService(mapperType);

        var method = mapperType.GetMethod(MapDisplayModelMethodName);
        if (method == null)
        {
            throw new Exception($"Could not find method {MapDisplayModelMethodName} on type {mapperType.FullName}");
        }

        var result = method.Invoke(mapper, new object[] { renderDetails, renderDetails.Model, publishStatusQuery });
        var typedResult = result as Task<TDisplayModel>;
        if (typedResult == null)
        {
            throw new Exception($"Unexpected result type when invoking {MapDisplayModelMethodName} on type {mapperType.FullName}. Expected Task of type '{typeof(TDisplayModel).FullName}' but got '{result?.GetType().FullName}'");
        }

        return await typedResult;
    }
}
