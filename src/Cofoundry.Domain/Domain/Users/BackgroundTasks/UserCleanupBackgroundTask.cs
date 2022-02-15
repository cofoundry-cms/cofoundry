using Cofoundry.Core.BackgroundTasks;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.BackgroundTasks
{
    public class UserCleanupBackgroundTask : IAsyncRecurringBackgroundTask
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public UserCleanupBackgroundTask(
            IDomainRepository domainRepository,
            IUserAreaDefinitionRepository userAreaDefinitionRepository
            )
        {
            _domainRepository = domainRepository;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
        }

        public async Task ExecuteAsync()
        {
            foreach (var userArea in _userAreaDefinitionRepository.GetAll())
            {
                var options = _userAreaDefinitionRepository.GetOptionsByCode(userArea.UserAreaCode).Cleanup;

                if (!options.Enabled)
                {
                    return;
                }

                await _domainRepository
                    .WithElevatedPermissions()
                    .ExecuteCommandAsync(new CleanupUsersCommand()
                    {
                        UserAreaCode = userArea.UserAreaCode,
                        DefaultRetentionPeriod = DaysToTimeSpan(options.DefaultRetentionPeriodInDays),
                        AuthenticationLogRetentionPeriod = DaysToTimeSpan(options.AuthenticationLogRetentionPeriodInDays),
                        AuthenticationFailLogRetentionPeriod = DaysToTimeSpan(options.AuthenticationFailLogRetentionPeriodInDays)
                    });
            }
        }

        private TimeSpan? DaysToTimeSpan(int? numDays)
        {
            if (!numDays.HasValue || numDays < 0) return null;

            return TimeSpan.FromDays(numDays.Value);
        }
    }
}