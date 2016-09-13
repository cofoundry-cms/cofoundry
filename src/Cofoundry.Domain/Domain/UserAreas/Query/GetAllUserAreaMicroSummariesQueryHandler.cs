using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using AutoMapper;

namespace Cofoundry.Domain
{
    public class GetAllUserAreaMicroSummariesQueryHandler 
        : IAsyncQueryHandler<GetAllQuery<UserAreaMicroSummary>, IEnumerable<UserAreaMicroSummary>>
        , ICofoundryUserPermissionCheckHandler
    {
        #region constructor

        private readonly IUserAreaRepository _userAreaRepository;
        private readonly IMapper _mapper;

        public GetAllUserAreaMicroSummariesQueryHandler(
            IUserAreaRepository userAreaRepository,
            IMapper mapper
            )
        {
            _userAreaRepository = userAreaRepository;
            _mapper = mapper;
        }

        #endregion

        #region execution

        public Task<IEnumerable<UserAreaMicroSummary>> ExecuteAsync(GetAllQuery<UserAreaMicroSummary> query, IExecutionContext executionContext)
        {
            var areas = _userAreaRepository.GetAll().OrderBy(u => u.Name);
            var results = Mapper.Map<IEnumerable<UserAreaMicroSummary>>(areas);

            return Task.FromResult(results);
        }

        #endregion
    }
}
