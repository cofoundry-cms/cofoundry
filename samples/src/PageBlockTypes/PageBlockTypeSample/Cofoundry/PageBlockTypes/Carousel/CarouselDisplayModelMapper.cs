using Cofoundry.Core;

namespace PageBlockTypeSample;

public class CarouselDisplayModelMapper : IPageBlockTypeDisplayModelMapper<CarouselDataModel>
{
    private readonly IContentRepository _contentRepository;

    public CarouselDisplayModelMapper(IContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
    }

    public async Task MapAsync(
        PageBlockTypeDisplayModelMapperContext<CarouselDataModel> context,
        PageBlockTypeDisplayModelMapperResult<CarouselDataModel> result)
    {
        // Find all the image ids to load
        var allImageAssetIds = context
            .Items
            .SelectMany(m => m.DataModel.Slides)
            .Select(m => m.ImageId)
            .Where(i => i > 0)
            .Distinct();

        // Load image data
        var allImages = await _contentRepository
            .WithContext(context.ExecutionContext)
            .ImageAssets()
            .GetByIdRange(allImageAssetIds)
            .AsRenderDetails()
            .ExecuteAsync();

        // Map display model
        foreach (var items in context.Items)
        {
            var slides = EnumerableHelper
                .Enumerate(items.DataModel.Slides)
                .Select(m => MapSlide(m, allImages))
                .WhereNotNull()
                .ToArray();

            var output = new CarouselDisplayModel
            {
                Slides = slides
            };

            result.Add(items, output);
        }
    }

    private static CarouselSlideDisplayModel? MapSlide(
        CarouselSlideDataModel dataModel,
        IReadOnlyDictionary<int, ImageAssetRenderDetails> allImages)
    {
        var image = allImages.GetValueOrDefault(dataModel.ImageId);

        if (image == null)
        {
            return null;
        }

        return new CarouselSlideDisplayModel()
        {
            Image = image,
            Text = dataModel.Text,
            Title = dataModel.Title
        };
    }
}
