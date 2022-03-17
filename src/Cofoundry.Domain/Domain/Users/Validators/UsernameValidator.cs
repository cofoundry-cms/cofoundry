using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain.Extendable;

/// <inheritdoc/>
public class UsernameValidator : IUsernameValidator
{
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
    private readonly IDomainRepository _domainRepository;

    public UsernameValidator(
        IUserAreaDefinitionRepository userAreaDefinitionRepository,
        IDomainRepository domainRepository
        )
    {
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
        _domainRepository = domainRepository;
    }

    public async virtual Task<ICollection<ValidationError>> GetErrorsAsync(IUsernameValidationContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        var validators = new List<Func<IUsernameValidationContext, ValidationError>>()
        {
            ValidateUsernameFormattingResult,
            ValidateMinLength,
            ValidateMaxLength,
            ValidateAllowedCharacters,
        };

        var asyncValidators = new List<Func<IUsernameValidationContext, Task<ValidationError>>>()
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
    /// Validates that the <see cref="IUsernameValidationContext.Username"/>
    /// property is not <see langword="null"/>, which indicates that the formatting
    /// operation did not return a value. This should rarely happen, only if the username 
    /// contains only characters stripped out through a custom normalization process.
    /// </summary>
    protected virtual ValidationError ValidateUsernameFormattingResult(IUsernameValidationContext context)
    {
        if (context.Username != null) return null;

        return UserValidationErrors
            .Username
            .InvalidCharacters
            .Create(context.PropertyName);
    }


    /// <summary>
    /// Validates that the username is not shorter then the limit defined in the 
    /// <see cref="UsernameOptions"/> configuration settings.
    /// </summary>
    protected virtual ValidationError ValidateMinLength(IUsernameValidationContext context)
    {
        var options = _userAreaDefinitionRepository.GetOptionsByCode(context.UserAreaCode).Username;
        if (context.Username.NormalizedUsername.Length >= options.MinLength) return null;

        return UserValidationErrors
            .Username
            .MinLengthNotMet
            .Customize()
            .WithMessageFormatParameters(options.MinLength)
            .WithProperties(context.PropertyName)
            .Create();
    }

    /// <summary>
    /// Validates that the username is not longer then the limit defined in the 
    /// <see cref="UsernameOptions"/> configuration settings.
    /// </summary>
    protected virtual ValidationError ValidateMaxLength(IUsernameValidationContext context)
    {
        var options = _userAreaDefinitionRepository.GetOptionsByCode(context.UserAreaCode).Username;
        if (context.Username.NormalizedUsername.Length <= options.MaxLength) return null;

        return UserValidationErrors
            .Username
            .MaxLengthExceeded
            .Customize()
            .WithMessageFormatParameters(options.MaxLength)
            .WithProperties(context.PropertyName)
            .Create();
    }

    /// <summary>
    /// Validates that the username contains only the characters permitted by the 
    /// <see cref="UsernameOptions"/> configuration settings.
    /// </summary>
    protected virtual ValidationError ValidateAllowedCharacters(IUsernameValidationContext context)
    {
        var userArea = _userAreaDefinitionRepository.GetRequiredByCode(context.UserAreaCode);

        // Bypass format validation for email-based usernames
        if (userArea.UseEmailAsUsername) return null;

        var options = _userAreaDefinitionRepository.GetOptionsByCode(context.UserAreaCode).Username;
        var invalidCharacters = UsernameCharacterValidator.GetInvalidCharacters(context.Username.NormalizedUsername, options);

        if (!invalidCharacters.Any()) return null;

        // Be careful here, because we're handling user input. Any message should be escaped when 
        // rendered, but to be safe we'll only include a single invalid character
        return UserValidationErrors
            .Username
            .InvalidCharacters
            .Customize()
            .WithMessageFormatParameters(invalidCharacters.First().ToString())
            .WithProperties(context.PropertyName)
            .Create();
    }

    /// <summary>
    /// Validates that the username is not already registered with another user.
    /// </summary>
    protected virtual async Task<ValidationError> ValidateUniqueAsync(IUsernameValidationContext context)
    {
        var query = new IsUsernameUniqueQuery()
        {
            Username = context.Username.UniqueUsername,
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
            .Username
            .NotUnique
            .Create(context.PropertyName);
    }

    private ICollection<ValidationError> WrapError(ValidationError error)
    {
        return new List<ValidationError>() { error };
    }
}
