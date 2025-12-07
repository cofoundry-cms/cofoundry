namespace Dev.Sandbox;

/// <summary>
/// This mapper is required to map from the data returned from the database to
/// a strongly typed model that we can use in the view template. This might seem
/// a little verbose but this allows us to use a strongly typed model in the view
/// and provides us with a lot of flexibility when mapping from unstructured data
/// </summary>
public class BlogPostDisplayModelMapper
    : ICustomEntityDisplayModelMapper<BlogPostDataModel, BlogPostDisplayModel>
{
    private readonly IContentRepository _contentRepository;

    public BlogPostDisplayModelMapper(
        IContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    /// <summary>
    /// Maps a raw custom entity data model to a display model that can be rendered out 
    /// to a view template.
    /// </summary>
    /// <param name="renderDetails">
    /// The raw custom entity data pulled from the database.
    /// </param>
    /// <param name="dataModel">
    /// Typed model data to map from. This is the same model that's in the render 
    /// details model, but is passed in as a typed model for easy access.
    /// </param>
    /// <param name="publishStatusQuery">
    /// The query type that should be used to query dependent entities. E.g. if the custom
    /// entity was queried with the Published status query, then any dependent entities should
    /// also be queried as Published.
    /// </param>
    public async Task<BlogPostDisplayModel> MapDisplayModelAsync(
        CustomEntityRenderDetails renderDetails,
        BlogPostDataModel dataModel,
        PublishStatusQuery publishStatusQuery
        )
    {
        var categories = await MapCategories(dataModel, publishStatusQuery);
        var author = await MapAuthor(dataModel, publishStatusQuery);

        var displayModel = new BlogPostDisplayModel()
        {
            MetaDescription = dataModel.ShortDescription,
            PageTitle = renderDetails.Title,
            Title = renderDetails.Title,
            Date = renderDetails.CreateDate,
            FullPath = renderDetails.PageUrls.FirstOrDefault() ?? string.Empty,
            Categories = categories,
            Author = author
        };

        return displayModel;
    }

    private async Task<IReadOnlyCollection<CategorySummary>> MapCategories(
        BlogPostDataModel dataModel,
        PublishStatusQuery publishStatusQuery
        )
    {
        if (EnumerableHelper.IsNullOrEmpty(dataModel.CategoryIds))
        {
            return Array.Empty<CategorySummary>();
        }

        // We manually query and map relations which gives us maximum flexibility when
        // mapping models. Cofoundry provides apis and extensions to make this easier.
        var results = await _contentRepository
            .CustomEntities()
            .GetByIdRange(dataModel.CategoryIds)
            .AsRenderSummaries(publishStatusQuery)
            .FilterAndOrderByKeys(dataModel.CategoryIds)
            .MapItem(MapCategory)
            .ExecuteAsync();

        return results;
    }

    /// <summary>
    /// We could use AutoMapper here, but to keep it simple let's just do manual mapping.
    /// </summary>
    private CategorySummary MapCategory(CustomEntityRenderSummary renderSummary)
    {
        // A CustomEntityRenderSummary will always contain the data model for the custom entity 
        var model = renderSummary.Model as CategoryDataModel;

        var category = new CategorySummary()
        {
            CategoryId = renderSummary.CustomEntityId,
            Title = renderSummary.Title,
            ShortDescription = model?.ShortDescription
        };

        return category;
    }

    private async Task<AuthorDetails?> MapAuthor(
        BlogPostDataModel dataModel,
        PublishStatusQuery publishStatusQuery
        )
    {
        if (dataModel.AuthorId < 1)
        {
            return null;
        }

        var dbAuthor = await _contentRepository
            .CustomEntities()
            .GetById(dataModel.AuthorId)
            .AsRenderSummary(publishStatusQuery)
            .ExecuteAsync();

        if (dbAuthor?.Model is not AuthorDataModel model)
        {
            return null;
        }

        var author = new AuthorDetails()
        {
            Name = dbAuthor.Title,
            Biography = HtmlFormatter.ConvertToBasicHtml(model.Biography)
        };

        if (!model.ProfileImageAssetId.HasValue)
        {
            return author;
        }

        author.ProfileImage = await _contentRepository
            .ImageAssets()
            .GetById(model.ProfileImageAssetId.Value)
            .AsRenderDetails()
            .ExecuteAsync();

        return author;
    }
}
