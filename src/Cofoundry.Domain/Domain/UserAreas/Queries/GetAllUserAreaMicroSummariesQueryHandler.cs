using Cofoundry.Domain.CQS;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class GetAllUserAreaMicroSummariesQueryHandler
        : IQueryHandler<GetAllUserAreaMicroSummariesQuery, ICollection<UserAreaMicroSummary>>
        , IIgnorePermissionCheckHandler
    {
        private readonly IUserAreaDefinitionRepository _userAreaRepository;

        public GetAllUserAreaMicroSummariesQueryHandler(
            IUserAreaDefinitionRepository userAreaRepository
            )
        {
            _userAreaRepository = userAreaRepository;
        }

        public Task<ICollection<UserAreaMicroSummary>> ExecuteAsync(GetAllUserAreaMicroSummariesQuery query, IExecutionContext executionContext)
        {
            var areas = _userAreaRepository.GetAll().OrderBy(u => u.Name);
            var results = areas
                .Select(a => new UserAreaMicroSummary()
                {
                    Name = a.Name,
                    UserAreaCode = a.UserAreaCode
                })
                .ToList();

            return Task.FromResult<ICollection<UserAreaMicroSummary>>(results);
        }
    }
}
