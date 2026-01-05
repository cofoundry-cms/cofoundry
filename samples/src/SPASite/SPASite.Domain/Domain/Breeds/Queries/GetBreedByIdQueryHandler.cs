namespace SPASite.Domain;

public class GetBreedByIdQueryHandler
    : IQueryHandler<GetBreedByIdQuery, Breed?>
    , IIgnorePermissionCheckHandler
{
    private readonly IContentRepository _contentRepository;

    public GetBreedByIdQueryHandler(IContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
    }

    public async Task<Breed?> ExecuteAsync(GetBreedByIdQuery query, IExecutionContext executionContext)
    {
        var breed = await _contentRepository
            .CustomEntities()
            .GetById(query.BreedId)
            .AsRenderSummary()
            .MapWhenNotNull(MapBreed)
            .ExecuteAsync();

        return breed;
    }

    /// <summary>
    /// For simplicity this logic is just repeated between handlers, but to 
    /// reduce repetition you could use a library like AutoMapper or break out
    /// the logic into a seperate mapper class and inject it in.
    /// </summary>
    private Breed? MapBreed(CustomEntityRenderSummary customEntity)
    {
        var breed = new Breed
        {
            BreedId = customEntity.CustomEntityId,
            Title = customEntity.Title
        };

        return breed;
    }
}
