using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Extendable;

/// <inheritdoc/>
public class EmailAddressValidator : IEmailAddressValidator
{
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
    private readonly IDomainRepository _domainRepository;

    public EmailAddressValidator(
        IUserAreaDefinitionRepository userAreaDefinitionRepository,
        IDomainRepository domainRepository
        )
    {
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
        _domainRepository = domainRepository;
    }

    public virtual async Task<ICollection<ValidationError>> GetErrorsAsync(IEmailAddressValidationContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var validators = new List<Func<IEmailAddressValidationContext, ValidationError>>()
        {
            ValidateEmailFormattingResult,
            ValidateMinLength,
            ValidateMaxLength,
            ValidateAllowedCharacters,
        };

        var asyncValidators = new List<Func<IEmailAddressValidationContext, Task<ValidationError>>>()
        {
            ValidateUniqueAsync
        };

        foreach (var validator in validators)
        {
            var error = validator(context);
            if (error != null) return WrapError(error);
        }

        foreach (var asyncValidator in asyncValidators)
        {
            var error = await asyncValidator(context);
            if (error != null) return WrapError(error);
        }

        return new List<ValidationError>();
    }

    /// <summary>
    /// Validates that the <see cref="IEmailAddressValidationContext.Email"/>
    /// property is not <see langword="null"/>, which indicates that the formatting
    /// operation did not return a value. This can happen if the normalizer was supplied
    /// a value but could not parse it successfully.
    /// </summary>
    protected virtual ValidationError ValidateEmailFormattingResult(IEmailAddressValidationContext context)
    {
        if (context.Email != null) return null;

        return UserValidationErrors
            .EmailAddress
            .InvalidFormat
            .Create(context.PropertyName);
    }

    /// <summary>
    /// Validates that the email is not shorter then the limit defined in the 
    /// <see cref="EmailAddressOptions"/> configuration settings.
    /// </summary>
    protected virtual ValidationError ValidateMinLength(IEmailAddressValidationContext context)
    {
        var options = _userAreaDefinitionRepository.GetOptionsByCode(context.UserAreaCode).EmailAddress;
        if (context.Email.NormalizedEmailAddress.Length >= options.MinLength) return null;

        return UserValidationErrors
            .EmailAddress
            .MinLengthNotMet
            .Customize()
            .WithMessageFormatParameters(options.MinLength)
            .WithProperties(context.PropertyName)
            .Create();
    }

    /// <summary>
    /// Validates that the email is not longer then the limit defined in the 
    /// <see cref="EmailAddressOptions"/> configuration settings.
    /// </summary>
    protected virtual ValidationError ValidateMaxLength(IEmailAddressValidationContext context)
    {
        var options = _userAreaDefinitionRepository.GetOptionsByCode(context.UserAreaCode).EmailAddress;
        if (context.Email.NormalizedEmailAddress.Length <= options.MaxLength) return null;

        return UserValidationErrors
            .EmailAddress
            .MaxLengthExceeded
            .Customize()
            .WithMessageFormatParameters(options.MaxLength)
            .WithProperties(context.PropertyName)
            .Create();
    }

    /// <summary>
    /// Validates that the email contains only the characters permitted by the 
    /// <see cref="EmailAddressOptions"/> configuration settings.
    /// </summary>
    protected virtual ValidationError ValidateAllowedCharacters(IEmailAddressValidationContext context)
    {
        var options = _userAreaDefinitionRepository.GetOptionsByCode(context.UserAreaCode).EmailAddress;
        var invalidCharacters = EmailAddressCharacterValidator.GetInvalidCharacters(context.Email.NormalizedEmailAddress, options);

        if (!invalidCharacters.Any()) return null;

        // Be careful here, because we're handling user input. Any message should be escaped when 
        // rendered, but to be safe we'll only include a single invalid character
        return UserValidationErrors
            .EmailAddress
            .InvalidCharacters
            .Customize()
            .WithMessageFormatParameters(invalidCharacters.First().ToString())
            .WithProperties(context.PropertyName)
            .Create();
    }

    /// <summary>
    /// Validates that the email is not already registered with another user.
    /// </summary>
    protected virtual async Task<ValidationError> ValidateUniqueAsync(IEmailAddressValidationContext context)
    {
        var userArea = _userAreaDefinitionRepository.GetRequiredByCode(context.UserAreaCode);
        var options = _userAreaDefinitionRepository.GetOptionsByCode(context.UserAreaCode).EmailAddress;
        if (!options.RequireUnique && !userArea.UseEmailAsUsername) return null;

        var query = new IsUserEmailAddressUniqueQuery()
        {
            Email = context.Email.UniqueEmailAddress,
            UserAreaCode = context.UserAreaCode
        };

        if (context.UserId.HasValue)
        {
            query.UserId = context.UserId.Value;
        }

        var isUnique = await _domainRepository
            .WithContext(context.ExecutionContext)
            .ExecuteQueryAsync(query);

        if (isUnique) return null;

        return UserValidationErrors
            .EmailAddress
            .NotUnique
            .Create(context.PropertyName);
    }

    private ICollection<ValidationError> WrapError(ValidationError error)
    {
        return new List<ValidationError>() { error };
    }
}
