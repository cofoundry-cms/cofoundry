using Cofoundry.Core;
using Cofoundry.Domain.Data;
using System;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class UserSummaryMapper : IUserSummaryMapper
    {
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IUserMicroSummaryMapper _userMicroSummaryMapper;
        private readonly IRoleMicroSummaryMapper _roleMicroSummaryMapper;

        public UserSummaryMapper(
            IUserAreaDefinitionRepository userAreaRepository,
            IUserMicroSummaryMapper userMicroSummaryMapper,
            IRoleMicroSummaryMapper roleMicroSummaryMapper
            )
        {
            _userAreaRepository = userAreaRepository;
            _userMicroSummaryMapper = userMicroSummaryMapper;
            _roleMicroSummaryMapper = roleMicroSummaryMapper;
        }

        public virtual UserSummary Map(User dbUser)
        {
            if (dbUser == null) return null;

            if (dbUser.Role == null)
            {
                throw new ArgumentException("dbUser.Role must be included in the query to map to use the UserSummaryMapper");
            }

            var user = _userMicroSummaryMapper.Map<UserSummary>(dbUser);
            user.Email = dbUser.Email;
            user.FirstName = dbUser.FirstName;
            user.LastName = dbUser.LastName;
            user.Username = dbUser.Username;
            user.LastSignInDate = dbUser.LastSignInDate;

            user.AuditData = new CreateAuditData()
            {
                CreateDate = dbUser.CreateDate
            };

            if (dbUser.Creator != null)
            {
                user.AuditData.Creator = _userMicroSummaryMapper.Map(dbUser.Creator);
            }

            var userArea = _userAreaRepository.GetRequiredByCode(dbUser.UserAreaCode);
            EntityNotFoundException.ThrowIfNull(userArea, dbUser.UserAreaCode);

            user.UserArea = new UserAreaMicroSummary()
            {
                UserAreaCode = dbUser.UserAreaCode,
                Name = userArea.Name
            };

            user.Role = _roleMicroSummaryMapper.Map(dbUser.Role);

            return user;
        }
    }
}