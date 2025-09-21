﻿namespace Cofoundry.BasicTestSite;

/// <summary>
/// This mapper is required to map from the data returned from the database to
/// a strongly typed model that we can use in the view template. This might seem
/// a little verbose but this allows us to use a strongly typed model in the view
/// and provides us with a lot of flexibility when mapping from unstructured data
/// </summary>
public class BlogPostDetailsDisplayModelMapper
    : ICustomEntityDisplayModelMapper<BlogPostDataModel, BlogPostDisplayModel>
{
    private readonly IContentRepository _contentRepository;

    public BlogPostDetailsDisplayModelMapper(
        IContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    public async Task<BlogPostDisplayModel> MapDisplayModelAsync(
        CustomEntityRenderDetails renderDetails,
        BlogPostDataModel dataModel,
        PublishStatusQuery publishStatusQuery
        )
    {
        var vm = new BlogPostDisplayModel
        {
            MetaDescription = dataModel.ShortDescription,
            PageTitle = renderDetails.Title,
            Title = renderDetails.Title,
            Date = renderDetails.CreateDate,
            FullPath = renderDetails.PageUrls.FirstOrDefault()
        };

        if (!EnumerableHelper.IsNullOrEmpty(dataModel.CategoryIds))
        {
            // We manually query and map relations which gives us maximum flexibility when mapping models
            // Fortunately the framework provides tools to make this fairly simple
            vm.Categories = await _contentRepository
                .CustomEntities()
                .GetByIdRange(dataModel.CategoryIds)
                .AsRenderSummaries(publishStatusQuery)
                .MapItem(MapCategory)
                .FilterAndOrderByKeys(dataModel.CategoryIds)
                .ExecuteAsync();
        }

        return vm;
    }

    /// <summary>
    /// We could use AutoMapper here, but to keep it simple let's just do manual mapping
    /// </summary>
    private CategorySummary MapCategory(CustomEntityRenderSummary renderSummary)
    {
        // A CustomEntityRenderSummary will always contain the data model for the custom entity 
        var model = renderSummary.Model as CategoryDataModel;

        var category = new CategorySummary
        {
            CategoryId = renderSummary.CustomEntityId,
            Title = renderSummary.Title,
            ShortDescription = model?.ShortDescription ?? string.Empty
        };

        return category;
    }
}
