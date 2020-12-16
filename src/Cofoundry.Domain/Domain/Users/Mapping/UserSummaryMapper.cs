using Cofoundry.Core;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to UserSummary objects.
    /// </summary>
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

        /// <summary>
        /// Maps an EF user record from the db into a UserSummary object. If the
        /// db record is null then null is returned.
        /// </summary>
        /// <param name="dbUser">User record from the database.</param>
        public virtual UserSummary Map(User dbUser)
        {
            if (dbUser == null) return null;

            if (dbUser.Role == null)
            {
                throw new ArgumentException("dbUser.Role must be included in the query to map to use the UserSummaryMapper");
            }

            var user = new UserSummary()
            {
                Email = dbUser.Email,
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                UserId = dbUser.UserId,
                Username = dbUser.Username,
                LastLoginDate = DbDateTimeMapper.AsUtc(dbUser.LastLoginDate)
            };

            user.AuditData = new CreateAuditData()
            {
                CreateDate = DbDateTimeMapper.AsUtc(dbUser.CreateDate)
            };

            if (dbUser.Creator != null)
            {
                user.AuditData.Creator = _userMicroSummaryMapper.Map(dbUser.Creator);
            }

            var userArea = _userAreaRepository.GetByCode(dbUser.UserAreaCode);
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
