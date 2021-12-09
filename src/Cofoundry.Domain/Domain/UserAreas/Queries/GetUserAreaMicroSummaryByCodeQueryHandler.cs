using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns a user area definition by it's unique <see cref="IUserAreaDefinition.UserAreaCode"/>. 
    /// If the definition does not exist then <see langword="null"/> is returned.
    /// </summary>
    public class GetUserAreaMicroSummaryByCodeQueryHandler
        : IQueryHandler<GetUserAreaMicroSummaryByCodeQuery, UserAreaMicroSummary>
        , IIgnorePermissionCheckHandler
    {
        private readonly IUserAreaDefinitionRepository _userAreaRepository;

        public GetUserAreaMicroSummaryByCodeQueryHandler(
            IUserAreaDefinitionRepository userAreaRepository
            )
        {
            _userAreaRepository = userAreaRepository;
        }

        public Task<UserAreaMicroSummary> ExecuteAsync(GetUserAreaMicroSummaryByCodeQuery query, IExecutionContext executionContext)
        {
            var areaDefinition = _userAreaRepository.GetByCode(query.UserAreaCode);
            UserAreaMicroSummary result = null;

            if (areaDefinition != null)
            {
                result = new UserAreaMicroSummary()
                {
                    Name = areaDefinition.Name,
                    UserAreaCode = areaDefinition.UserAreaCode
                };
            }

            return Task.FromResult(result);
        }
    }
}
