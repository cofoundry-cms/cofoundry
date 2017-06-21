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

        public GetAllUserAreaMicroSummariesQueryHandler(
            IUserAreaRepository userAreaRepository
            )
        {
            _userAreaRepository = userAreaRepository;
        }

        #endregion

        #region execution

        public Task<IEnumerable<UserAreaMicroSummary>> ExecuteAsync(GetAllQuery<UserAreaMicroSummary> query, IExecutionContext executionContext)
        {
            var areas = _userAreaRepository.GetAll().OrderBy(u => u.Name);
            var results = areas
                .Select(a => new UserAreaMicroSummary()
                {
                    Name = a.Name,
                    UserAreaCode = a.UserAreaCode
                });

            return Task.FromResult(results);
        }

        #endregion
    }
}
