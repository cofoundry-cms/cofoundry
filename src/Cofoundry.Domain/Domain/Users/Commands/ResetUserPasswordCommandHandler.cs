using Cofoundry.Core;
using Cofoundry.Core.Mail;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.MailTemplates;
using Microsoft.AspNetCore.Html;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Resets a users password to a randomly generated temporary value
    /// and sends it in a mail a notification to the user. The password
    /// will need to be changed at first login (if the user area supports 
    /// it). This is designed to be used from an admin screen rather than 
    /// a self-service reset which can be done via 
    /// <see cref="InitiateUserAccountRecoveryByEmailCommand"/>.
    /// </summary>
    public class ResetUserPasswordCommandHandler
        : ICommandHandler<ResetUserPasswordCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IMailService _mailService;
        private readonly IDomainRepository _domainRepository;
        private readonly IUserMailTemplateBuilderFactory _userMailTemplateBuilderFactory;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IPasswordCryptographyService _passwordCryptographyService;
        private readonly IPasswordGenerationService _passwordGenerationService;
        private readonly IUserSecurityStampUpdateHelper _userSecurityStampUpdateHelper;
        private readonly IUserContextCache _userContextCache;
        private readonly IMessageAggregator _messageAggregator;

        public ResetUserPasswordCommandHandler(
            CofoundryDbContext dbContext,
            IMailService mailService,
            IDomainRepository domainRepository,
            IUserMailTemplateBuilderFactory userMailTemplateBuilderFactory,
            IPermissionValidationService permissionValidationService,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IPasswordCryptographyService passwordCryptographyService,
            IPasswordGenerationService passwordGenerationService,
            IUserSecurityStampUpdateHelper userSecurityStampUpdateHelper,
            IUserContextCache userContextCache,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _mailService = mailService;
            _domainRepository = domainRepository;
            _userMailTemplateBuilderFactory = userMailTemplateBuilderFactory;
            _permissionValidationService = permissionValidationService;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _passwordCryptographyService = passwordCryptographyService;
            _passwordGenerationService = passwordGenerationService;
            _userSecurityStampUpdateHelper = userSecurityStampUpdateHelper;
            _userContextCache = userContextCache;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(ResetUserPasswordCommand command, IExecutionContext executionContext)
        {
            var user = await GetUserAsync(command.UserId);
            ValidatePermissions(user, executionContext);
            ValidateUserArea(user.UserAreaCode);

            var options = _userAreaDefinitionRepository.GetOptionsByCode(user.UserAreaCode);
            var temporaryPassword = _passwordGenerationService.Generate(options.Password.MinLength);

            var hashResult = _passwordCryptographyService.CreateHash(temporaryPassword);
            user.Password = hashResult.Hash;
            user.PasswordHashVersion = hashResult.HashVersion;
            user.RequirePasswordChange = true;
            user.LastPasswordChangeDate = executionContext.ExecutionDate;
            _userSecurityStampUpdateHelper.Update(user);

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                await _dbContext.SaveChangesAsync();
                await SendNotificationAsync(user, temporaryPassword, executionContext);

                scope.QueueCompletionTask(() => OnTransactionComplete(user));
                await scope.CompleteAsync();
            }
        }

        private async Task OnTransactionComplete(User user)
        {
            _userContextCache.Clear();

            await _userSecurityStampUpdateHelper.OnTransactionCompleteAsync(user);

            await _messageAggregator.PublishAsync(new UserPasswordResetMessage()
            {
                UserAreaCode = user.UserAreaCode,
                UserId = user.UserId
            });
        }

        private Task<User> GetUserAsync(int userId)
        {
            var user = _dbContext
                .Users
                .FilterById(userId)
                .FilterCanLogIn()
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(user, userId);

            return user;
        }

        private void ValidateUserArea(string userAreaCode)
        {
            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(userAreaCode);

            if (!userArea.AllowPasswordLogin)
            {
                throw new InvalidOperationException($"Cannot reset the password because the {userArea.Name} user area does not allow password logins.");
            }

            if (!userArea.UseEmailAsUsername)
            {
                throw new InvalidOperationException($"Cannot reset the password because the {userArea.Name} user area does not require email addresses.");
            }
        }

        private async Task SendNotificationAsync(User user, string temporaryPassword, IExecutionContext executionContext)
        {
            // Send mail notification
            var mailTemplateBuilder = _userMailTemplateBuilderFactory.Create(user.UserAreaCode);

            var context = await CreateMailTemplateContextAsync(user, temporaryPassword, executionContext);
            var mailTemplate = await mailTemplateBuilder.BuildPasswordResetTemplateAsync(context);

            // Null template means don't send a notification
            if (mailTemplate == null) return;

            await _mailService.SendAsync(user.Email, mailTemplate);
        }

        private async Task<PasswordResetTemplateBuilderContext> CreateMailTemplateContextAsync(
            User user,
            string temporaryPassword,
            IExecutionContext executionContext
            )
        {
            var query = new GetUserSummaryByIdQuery(user.UserId);
            var userSummary = await _domainRepository
                .WithContext(executionContext)
                .ExecuteQueryAsync(query);
            EntityNotFoundException.ThrowIfNull(userSummary, user.UserId);

            var context = new PasswordResetTemplateBuilderContext()
            {
                User = userSummary,
                TemporaryPassword = new HtmlString(temporaryPassword)
            };

            return context;
        }

        public void ValidatePermissions(User user, IExecutionContext executionContext)
        {
            if (user.UserId == executionContext.UserContext.UserId)
            {
                throw new NotPermittedException("A user cannot reset the password on their own user account.");
            }

            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(user.UserAreaCode);
            if (userArea is CofoundryAdminUserArea)
            {
                _permissionValidationService.EnforcePermission(new CofoundryUserResetPasswordPermission(), executionContext.UserContext);
            }
            else
            {
                _permissionValidationService.EnforcePermission(new NonCofoundryUserResetPasswordPermission(), executionContext.UserContext);
            }
        }
    }
}
