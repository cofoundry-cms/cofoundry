using Cofoundry.Core;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to UserMicroSummary objects.
    /// </summary>
    public class UserMicroSummaryMapper : IUserMicroSummaryMapper
    {
        private readonly IUserAreaDefinitionRepository _userAreaRepository;

        public UserMicroSummaryMapper(IUserAreaDefinitionRepository userAreaRepository)
        {
            _userAreaRepository = userAreaRepository;
        }

        /// <summary>
        /// Maps an EF user record from the db into a UserMicroSummary object. If the
        /// db record is null then null is returned.
        /// </summary>
        /// <param name="dbUser">User record from the database.</param>
        public virtual UserMicroSummary Map(User dbUser)
        {
            if (dbUser == null) return null;

            var user = new UserMicroSummary()
            {
                Email = dbUser.Email,
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                UserId = dbUser.UserId,
                Username = dbUser.Username
            };

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
