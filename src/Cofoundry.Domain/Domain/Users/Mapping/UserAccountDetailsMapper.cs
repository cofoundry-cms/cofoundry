using Cofoundry.Core;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to UserAccountDetails objects.
    /// </summary>
    public class UserAccountDetailsMapper : IUserAccountDetailsMapper
    {
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IUserMicroSummaryMapper _userMicroSummaryMapper;

        public UserAccountDetailsMapper(
            IUserAreaDefinitionRepository userAreaRepository,
            IUserMicroSummaryMapper userMicroSummaryMapper
            )
        {
            _userAreaRepository = userAreaRepository;
            _userMicroSummaryMapper = userMicroSummaryMapper;
        }

        /// <summary>
        /// Maps an EF user record from the db into a UserAccountDetails object. If the
        /// db record is null then null is returned.
        /// </summary>
        /// <param name="dbUser">User record from the database.</param>
        public virtual UserAccountDetails Map(User dbUser)
        {
            if (dbUser == null) return null;

            var user = new UserAccountDetails()
            {
                Email = dbUser.Email,
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                UserId = dbUser.UserId,
                Username = dbUser.Username,
                LastLoginDate = DbDateTimeMapper.AsUtc(dbUser.LastLoginDate),
                LastPasswordChangeDate = DbDateTimeMapper.AsUtc(dbUser.LastPasswordChangeDate),
                PreviousLoginDate = DbDateTimeMapper.AsUtc(dbUser.PreviousLoginDate),
                RequirePasswordChange = dbUser.RequirePasswordChange
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

            return user;
        }
    }
}
