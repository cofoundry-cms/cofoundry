using Cofoundry.Core;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data.Internal;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Returns <see langword="true"/> if the parameters of the authentication
    /// attempt exceed the limits sets in configuration e.g. attempts per IP Address
    /// or per username.
    /// </summary>
    public class HasExceededMaxAuthenticationAttemptsQueryHandler
        : IQueryHandler<HasExceededMaxAuthenticationAttemptsQuery, bool>
        , IIgnorePermissionCheckHandler
    {
        private readonly IUserStoredProcedures _userStoredProcedures;
        private readonly IClientConnectionService _clientConnectionService;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

        public HasExceededMaxAuthenticationAttemptsQueryHandler(
            IUserStoredProcedures userStoredProcedures,
            IClientConnectionService clientConnectionService,
            IUserAreaDefinitionRepository userAreaDefinitionRepository
            )
        {
            _userStoredProcedures = userStoredProcedures;
            _clientConnectionService = clientConnectionService;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
        }

        public async Task<bool> ExecuteAsync(HasExceededMaxAuthenticationAttemptsQuery query, IExecutionContext executionContext)
        {
            var options = _userAreaDefinitionRepository.GetOptionsByCode(query.UserAreaCode).Authentication;

            if ((options.IPAddressRateLimit == null || !options.IPAddressRateLimit.HasValidQuantityAndWindow()) 
                && (options.UsernameRateLimit == null || !options.UsernameRateLimit.HasValidQuantityAndWindow()))
            {
                return false;
            }

            var connectionInfo = _clientConnectionService.GetConnectionInfo();

            var isValid = await _userStoredProcedures.IsAuthenticationAttemptValidAsync(
                query.UserAreaCode,
                TextFormatter.Limit(query.Username, 150),
                 connectionInfo.IPAddress,
                 executionContext.ExecutionDate,
                GetRateLimitQuantityIfValid(options.IPAddressRateLimit),
                RateLimitWindowToSeconds(options.IPAddressRateLimit),
                GetRateLimitQuantityIfValid(options.UsernameRateLimit),
                RateLimitWindowToSeconds(options.UsernameRateLimit)
                );

            return !isValid;
        }

        private int? GetRateLimitQuantityIfValid(RateLimitConfiguration rateLimit)
        {
            if (rateLimit == null || !rateLimit.HasValidQuantityAndWindow()) return null;

            return rateLimit.Quantity;
        }

        private int? RateLimitWindowToSeconds(RateLimitConfiguration rateLimit)
        {
            if (rateLimit == null || !rateLimit.HasValidQuantityAndWindow()) return null;

            if (rateLimit.Window.TotalSeconds > Int32.MaxValue)
            {
                throw new InvalidOperationException("Invalid rate limiting window. The number of seconds cannot exceed Int32.MaxValue.");
            }

            return Convert.ToInt32(rateLimit.Window.TotalSeconds);
        }
    }
}