namespace Cofoundry.Domain.Internal;

public class GetAllUserAreaMicroSummariesQueryHandler
    : IQueryHandler<GetAllUserAreaMicroSummariesQuery, IReadOnlyCollection<UserAreaMicroSummary>>
    , IIgnorePermissionCheckHandler
{
    private readonly IUserAreaDefinitionRepository _userAreaRepository;

    public GetAllUserAreaMicroSummariesQueryHandler(
        IUserAreaDefinitionRepository userAreaRepository
        )
    {
        _userAreaRepository = userAreaRepository;
    }

    public Task<IReadOnlyCollection<UserAreaMicroSummary>> ExecuteAsync(GetAllUserAreaMicroSummariesQuery query, IExecutionContext executionContext)
    {
        var areas = _userAreaRepository.GetAll().OrderBy(u => u.Name);
        var results = areas
            .Select(a => new UserAreaMicroSummary()
            {
                Name = a.Name,
                UserAreaCode = a.UserAreaCode
            })
            .ToArray();

        return Task.FromResult<IReadOnlyCollection<UserAreaMicroSummary>>(results);
    }
}
