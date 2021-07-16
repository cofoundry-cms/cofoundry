using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// In-memory implementation of IUserSessionService for non-web
    /// scenarios.
    /// </summary>
    public class InMemoryUserSessionService : IUserSessionService
    {
        private const string DEFAULT_USER_AREA_KEY = "DEFAULT_KEY";
        private Dictionary<string, int?> _userIdCache = new Dictionary<string, int?>();
        private object _lock = new object();

        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public InMemoryUserSessionService(
            IUserAreaDefinitionRepository userAreaDefinitionRepository
            )
        {
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
        }

        /// <summary>
        /// Gets the UserId of the user authenticated for the
        /// default authentication user area.
        /// </summary>
        /// <returns>
        /// Integer UserId or null if the user is not logged in for the default
        /// user area.
        /// </returns>
        public int? GetCurrentUserId()
        {
            return _userIdCache.GetValueOrDefault(DEFAULT_USER_AREA_KEY);
        }

        /// <summary>
        /// Gets the UserId of the currently logged in user for a specific UserArea. Useful in multi-userarea
        /// scenarios where you need to ignore the default user and check for permissions 
        /// against a specific user area.
        /// </summary>
        /// <param name="userAreaCode">The unique identifying code of the user area to check for.</param>
        public Task<int?> GetUserIdByUserAreaCodeAsync(string userAreaCode)
        {
            var result = GetUserIdByUserAreaCode(userAreaCode);
            return Task.FromResult(result);
        }

        /// <summary>
        /// Gets the UserId of the currently logged in user for a specific UserArea. Useful in multi-userarea
        /// scenarios where you need to ignore the default user and check for permissions 
        /// against a specific user area.
        /// </summary>
        /// <param name="userAreaCode">The unique identifying code of the user area to check for.</param>
        public int? GetUserIdByUserAreaCode(string userAreaCode)
        {
            if (userAreaCode == null)
            {
                throw new ArgumentNullException(nameof(userAreaCode));
            }

            return _userIdCache.GetValueOrDefault(userAreaCode);
        }

        /// <summary>
        /// Logs the specified UserId into the current session.
        /// </summary>
        /// <param name="userAreaCode">Unique code of the user area to log the user into (required).</param>
        /// <param name="userId">UserId belonging to the owner of the current session.</param>
        /// <param name="rememberUser">
        /// This value is ignored for the in-memory store and the value is stored for the 
        /// lifecycle of the service.
        /// </param>
        public Task LogUserInAsync(string userAreaCode, int userId, bool rememberUser)
        {
            if (userAreaCode == null) throw new ArgumentNullException(nameof(userAreaCode));
            if (userId < 1) throw new ArgumentOutOfRangeException(nameof(userId));

            var userArea = _userAreaDefinitionRepository.GetByCode(userAreaCode);
            EntityNotFoundException.ThrowIfNull(userArea, userAreaCode);
            var isDefaultUserArea = IsDefaultUserArea(userArea);

            lock (_lock)
            {
                _userIdCache[userArea.UserAreaCode] = userId;

                if (isDefaultUserArea)
                {
                    _userIdCache[DEFAULT_USER_AREA_KEY] = userId;
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Logs the user out of the specified user area.
        /// </summary>
        /// <param name="userAreaCode">Unique code of the user area to log the user out of (required).</param>
        public Task LogUserOutAsync(string userAreaCode)
        {
            if (userAreaCode == null)
            {
                throw new ArgumentNullException(nameof(userAreaCode));
            }

            var userArea = _userAreaDefinitionRepository.GetByCode(userAreaCode);
            EntityNotFoundException.ThrowIfNull(userArea, userAreaCode);
            var isDefaultUserArea = IsDefaultUserArea(userArea);

            lock (_lock)
            {
                _userIdCache[userArea.UserAreaCode] = null;

                if (isDefaultUserArea)
                {
                    _userIdCache[DEFAULT_USER_AREA_KEY] = null;
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Logs the user out of all user areas.
        /// </summary>
        public Task LogUserOutOfAllUserAreasAsync()
        {
            lock (_lock)
            {
                _userIdCache.Clear();
            }

            return Task.CompletedTask;
        }

        private bool IsDefaultUserArea(IUserAreaDefinition userArea)
        {
            var defaultUserArea = _userAreaDefinitionRepository.GetDefault();
            EntityNotFoundException.ThrowIfNull(defaultUserArea, "Default");

            var isDefault = userArea.UserAreaCode == defaultUserArea.UserAreaCode;
            return isDefault;
        }
    }
}
