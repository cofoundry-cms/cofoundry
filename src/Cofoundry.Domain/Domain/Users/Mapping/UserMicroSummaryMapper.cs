using Cofoundry.Core;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class UserMicroSummaryMapper : IUserMicroSummaryMapper
    {
        private readonly IUserAreaDefinitionRepository _userAreaRepository;

        public UserMicroSummaryMapper(
            IUserAreaDefinitionRepository userAreaRepository
            )
        {
            _userAreaRepository = userAreaRepository;
        }

        public virtual TModel Map<TModel>(User dbUser)
            where TModel : UserMicroSummary, new()
        {
            if (dbUser == null) return null;

            var user = new TModel()
            {
                UserId = dbUser.UserId,
                DisplayName = dbUser.DisplayName,
                AccountStatus = MapAccountStatus(dbUser)
            };

            var userArea = _userAreaRepository.GetRequiredByCode(dbUser.UserAreaCode);
            EntityNotFoundException.ThrowIfNull(userArea, dbUser.UserAreaCode);

            user.UserArea = new UserAreaMicroSummary()
            {
                UserAreaCode = dbUser.UserAreaCode,
                Name = userArea.Name
            };

            return user;
        }

        private UserAccountStatus MapAccountStatus(User dbUser)
        {
            if (dbUser.DeletedDate.HasValue)
            {
                return UserAccountStatus.Deleted;
            }
            else if (dbUser.DeactivatedDate.HasValue)
            {
                return UserAccountStatus.Deactivated;
            }

            return UserAccountStatus.Active;
        }

        public virtual UserMicroSummary Map(User dbUser)
        {
            return Map<UserMicroSummary>(dbUser);
        }
    }
}