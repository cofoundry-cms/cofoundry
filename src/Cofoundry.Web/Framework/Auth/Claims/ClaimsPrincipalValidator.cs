using Cofoundry.Core;
using Cofoundry.Core.Time;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Cofoundry.Web.Internal
{
    /// <inheritdoc/>
    /// <remarks>
    /// Adapted from <see cref="Microsoft.AspNetCore.Identity.SecurityStampValidator"/>,
    /// see https://github.com/dotnet/aspnetcore/blob/v6.0.1/src/Identity/Core/src/SecurityStampValidator.cs
    /// </remarks>
    public class ClaimsPrincipalValidator : IClaimsPrincipalValidator
    {
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IDateTimeService _dateTimeService;
        private readonly IUserSessionService _userSessionService;
        private readonly IClaimsPrincipalFactory _claimsPrincipalFactory;
        private readonly IClaimsPrincipalBuilderContextRepository _claimsPrincipalBuilderContextRepository;
        private readonly ILogger<ClaimsPrincipalValidator> _logger;

        public ClaimsPrincipalValidator(
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IDateTimeService dateTimeService,
            IUserSessionService userSessionService,
            IClaimsPrincipalFactory claimsPrincipalFactory,
            IClaimsPrincipalBuilderContextRepository claimsPrincipalBuilderContextRepository,
            ILogger<ClaimsPrincipalValidator> logger
            )
        {
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _dateTimeService = dateTimeService;
            _userSessionService = userSessionService;
            _claimsPrincipalFactory = claimsPrincipalFactory;
            _claimsPrincipalBuilderContextRepository = claimsPrincipalBuilderContextRepository;
            _logger = logger;
        }

        public virtual async Task ValidateAsync(CookieValidatePrincipalContext validationContext)
        {
            var now = _dateTimeService.OffsetUtcNow();
            var issuedUtc = validationContext.Properties.IssuedUtc;

            var validate = issuedUtc == null;
            if (issuedUtc != null)
            {
                var validationInterval = GetClaimsValidationInterval(validationContext);
                var timeElapsed = now.Subtract(issuedUtc.Value);
                validate = timeElapsed > validationInterval;
            }

            if (validate)
            {
                var claimsPrincipalBuilderContext = await VerifyClaimsPrincipalAsync(validationContext.Principal);
                if (claimsPrincipalBuilderContext != null)
                {
                    await ClaimsPrincipalVerifiedAsync(claimsPrincipalBuilderContext, validationContext);
                }
                else
                {
                    await ClaimsPrincipalRejectedAsync(validationContext);
                }
            }
        }

        private TimeSpan GetClaimsValidationInterval(CookieValidatePrincipalContext validationContext)
        {
            var userAreaCode = validationContext.Principal.FindFirstValue(CofoundryClaimTypes.UserAreaCode);

            if (!_userAreaDefinitionRepository.Exists(userAreaCode))
            {
                // Invalid or missing user area claim: the cookie should be invalidated immediately
                _logger.LogInformation("Invalid or missing user area claim: {userAreaCode}.", userAreaCode);
                return TimeSpan.Zero;
            }

            var options = _userAreaDefinitionRepository.GetOptionsByCode(userAreaCode);
            return options.Cookies.ClaimsValidationInterval;
        }


        /// <summary>
        /// Verifies the security stamp claim in the specified <paramref name="principal"/>. If the validation
        /// if successful then a <see cref="IClaimsPrincipalBuilderContext"/> instance is returned which can be 
        /// used to refresh the principal. If the stamp is invalid for any reason then <see langword="null"/>
        /// is returned.
        /// </summary>
        /// <param name="principal">The principal to verify.</param>
        protected virtual async Task<IClaimsPrincipalBuilderContext> VerifyClaimsPrincipalAsync(ClaimsPrincipal principal)
        {
            if (principal == null) return null;

            var userId = IntParser.ParseOrDefault(principal.FindFirstValue(CofoundryClaimTypes.UserId));
            var context = await _claimsPrincipalBuilderContextRepository.GetAsync(userId);
            if (context == null) return null;

            var securityStampClaim = principal.FindFirstValue(CofoundryClaimTypes.SecurityStamp);
            if (string.IsNullOrEmpty(context.SecurityStamp) || securityStampClaim == context.SecurityStamp)
            {
                return context;
            }

            _logger.LogDebug("Security stamp validation failed.");
            return null;
        }

        /// <summary>
        /// Called when the security stamp has been verified. By default this
        /// refreshes the claim principal.
        /// </summary>
        /// <remarks>
        /// The <see cref="Microsoft.AspNetCore.Identity.SecurityStampValidator"/> that this is 
        /// based on refreshed the principal if it is valid. Since the only claim we store that 
        /// changes is the security stamp, this is uneccessary for us, however I'm leaving it in
        /// as claims refreshing may be required in the future, or if anyone does decide to
        /// update the <see cref="IClaimsPrincipalFactory"/> to include data that needs to be 
        /// regularly updated.
        /// </remarks>
        /// <param name="claimsPrincipalBuilderContext">
        /// A context object representing the verified user. This can
        /// be used to build the refreshed claims principal.
        /// </param>
        /// <param name="context">A context object representing the parameters of the cookie validation event.</param>
        protected virtual async Task ClaimsPrincipalVerifiedAsync(IClaimsPrincipalBuilderContext claimsPrincipalBuilderContext, CookieValidatePrincipalContext validationContext)
        {
            var newPrincipal = await _claimsPrincipalFactory.CreateAsync(claimsPrincipalBuilderContext);

            validationContext.ReplacePrincipal(newPrincipal);
            validationContext.ShouldRenew = true;

            if (!validationContext.Options.SlidingExpiration)
            {
                // On renewal calculate the new ticket length relative to now to avoid
                // extending the expiration.
                validationContext.Properties.IssuedUtc = _dateTimeService.OffsetUtcNow();
            }
        }

        /// <summary>
        /// Called when the claims principal is invalid and needs to be rejected. By default this
        /// calls <see cref="CookieValidatePrincipalContext.RejectPrincipal"/> and then clears
        /// the session in the <see cref="IUserSessionService"/>.
        /// </summary>
        /// <param name="validationContext">A context object representing the parameters of the cookie validation event.</param>
        protected virtual async Task ClaimsPrincipalRejectedAsync(CookieValidatePrincipalContext validationContext)
        {
            _logger.LogDebug("Security stamp validation failed, rejecting cookie.");
            var userAreaCode = validationContext.Principal.FindFirstValue(CofoundryClaimTypes.UserAreaCode);
            validationContext.RejectPrincipal();

            if (_userAreaDefinitionRepository.Exists(userAreaCode))
            {
                await _userSessionService.SignOutAsync(userAreaCode);
            }
        }
    }
}
