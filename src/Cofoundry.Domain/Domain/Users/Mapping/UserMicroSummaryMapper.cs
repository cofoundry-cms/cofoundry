using Cofoundry.Core;
using Cofoundry.Domain.Data;
using Microsoft.Extensions.Logging;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class UserMicroSummaryMapper : IUserMicroSummaryMapper
    {
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly ILogger<UserMicroSummaryMapper> _logger;

        public UserMicroSummaryMapper(
            IUserAreaDefinitionRepository userAreaRepository,
            ILogger<UserMicroSummaryMapper> logger
            )
        {
            _userAreaRepository = userAreaRepository;
            _logger = logger;
        }

        public virtual TModel Map<TModel>(User dbUser)
            where TModel : UserMicroSummary, new()
        {
            if (dbUser == null) return null;

            var user = new TModel()
            {
                Email = dbUser.Email,
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                UserId = dbUser.UserId,
                Username = dbUser.Username,
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
            if (dbUser.IsDeleted)
            {
                return UserAccountStatus.Deleted;
            }
            else if (!dbUser.IsActive)
            {
                return UserAccountStatus.Deactivated;
            }
            else if (dbUser.IsActive)
            {
                return UserAccountStatus.Active;
            }

            _logger.LogWarning("Unknown UserAccountStatus when mapping user {UserId}", dbUser.UserId);
            return UserAccountStatus.Unknown;
        }

        public virtual UserMicroSummary Map(User dbUser)
        {
            return Map<UserMicroSummary>(dbUser);
        }
    }
}
