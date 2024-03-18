﻿using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class UserDataFormatter : IUserDataFormatter
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;

    public UserDataFormatter(
        IServiceProvider serviceProvider,
        IUserAreaDefinitionRepository userAreaDefinitionRepository
        )
    {
        _serviceProvider = serviceProvider;
        _userAreaDefinitionRepository = userAreaDefinitionRepository;
    }

    public EmailAddressFormattingResult? FormatEmailAddress(IUserAreaDefinition userAreaDefinition, string? emailAddress)
    {
        var emailAddressNormalizer = CreateServiceForUserArea<IEmailAddressNormalizer>(userAreaDefinition, typeof(IEmailAddressNormalizer<>));
        var normalized = emailAddressNormalizer.NormalizeAsParts(emailAddress);
        if (normalized == null) return null;

        var emailAddressUniquifier = CreateServiceForUserArea<IEmailAddressUniquifier>(userAreaDefinition, typeof(IEmailAddressUniquifier<>));
        var uniquified = emailAddressUniquifier.UniquifyAsParts(normalized);
        if (uniquified == null) return null;

        var result = new EmailAddressFormattingResult()
        {
            NormalizedEmailAddress = normalized.ToEmailAddress(),
            UniqueEmailAddress = uniquified.ToEmailAddress(),
            Domain = uniquified.Domain,
        };

        return result;
    }

    public UsernameFormattingResult? FormatUsername(IUserAreaDefinition userAreaDefinition, EmailAddressFormattingResult? emailAddress)
    {
        ArgumentNullException.ThrowIfNull(userAreaDefinition);
        if (!userAreaDefinition.UseEmailAsUsername)
        {
            throw new InvalidOperationException($"{nameof(FormatUsername)} can only be called with an email address if the user area supports using an email as a username.");
        }

        if (emailAddress == null) return null;

        var usernameNormalizer = CreateServiceForUserArea<IUsernameNormalizer>(userAreaDefinition, typeof(IUsernameNormalizer<>));
        var usernameUniquifier = CreateServiceForUserArea<IUsernameUniquifier>(userAreaDefinition, typeof(IUsernameUniquifier<>));

        var normalizedUsername = usernameNormalizer.Normalize(emailAddress.NormalizedEmailAddress);
        if (normalizedUsername == null) return null;

        var uniqueUsername = usernameUniquifier.Uniquify(emailAddress.NormalizedEmailAddress);
        if (uniqueUsername == null) return null;

        var result = new UsernameFormattingResult()
        {
            NormalizedUsername = normalizedUsername,
            UniqueUsername = uniqueUsername
        };

        return result;
    }

    public UsernameFormattingResult? FormatUsername(IUserAreaDefinition userAreaDefinition, string? username)
    {
        var usernameNormalizer = CreateServiceForUserArea<IUsernameNormalizer>(userAreaDefinition, typeof(IUsernameNormalizer<>));
        var usernameUniquifier = CreateServiceForUserArea<IUsernameUniquifier>(userAreaDefinition, typeof(IUsernameUniquifier<>));

        if (userAreaDefinition.UseEmailAsUsername)
        {
            var emailAddressNormalizer = CreateServiceForUserArea<IEmailAddressNormalizer>(userAreaDefinition, typeof(IEmailAddressNormalizer<>));
            username = emailAddressNormalizer.Normalize(username);
        }

        var normalizedUsername = usernameNormalizer.Normalize(username);
        if (normalizedUsername == null) return null;

        var uniqueUsername = usernameUniquifier.Uniquify(username);
        if (uniqueUsername == null) return null;

        var result = new UsernameFormattingResult()
        {
            NormalizedUsername = normalizedUsername,
            UniqueUsername = uniqueUsername
        };

        return result;
    }

    public string? FormatUsernameForLookup(string userAreaCode, string? username)
    {
        var userAreaDefinition = _userAreaDefinitionRepository.GetRequiredByCode(userAreaCode);
        var usernameUniquifier = CreateServiceForUserArea<IUsernameUniquifier>(userAreaCode, typeof(IUsernameUniquifier<>));

        if (userAreaDefinition.UseEmailAsUsername)
        {
            var emailAddressNormalizer = CreateServiceForUserArea<IEmailAddressNormalizer>(userAreaDefinition, typeof(IEmailAddressNormalizer<>));
            username = emailAddressNormalizer.Normalize(username);
        }

        var result = usernameUniquifier.Uniquify(username);

        return result;
    }

    public string? FormatEmailAddressForLookup(string userAreaCode, string? emailAddress)
    {
        var emailUniquifier = CreateServiceForUserArea<IUsernameUniquifier>(userAreaCode, typeof(IUsernameUniquifier<>));
        var result = emailUniquifier.Uniquify(emailAddress);

        return result;
    }

    public virtual string? NormalizeEmail(string userAreaDefinitionCode, string? emailAddress)
    {
        var service = CreateServiceForUserArea<IEmailAddressNormalizer>(userAreaDefinitionCode, typeof(IEmailAddressNormalizer<>));
        return service.Normalize(emailAddress);
    }

    //public virtual string NormalizeEmail(IUserAreaDefinition userAreaDefinition, string emailAddress)
    //{
    //    var service = CreateServiceForUserArea<IEmailAddressNormalizer>(userAreaDefinition, typeof(IEmailAddressNormalizer<>));
    //    return service.Normalize(emailAddress);
    //}

    //public virtual string UniquifyEmail(IUserAreaDefinition userAreaDefinition, string emailAddress)
    //{
    //    var service = CreateServiceForUserArea<IEmailAddressUniquifier>(userAreaDefinition, typeof(IEmailAddressUniquifier<>));
    //    return service.Uniquify(emailAddress);
    //}

    public virtual string? UniquifyEmail(string userAreaDefinitionCode, string? emailAddress)
    {
        var service = CreateServiceForUserArea<IEmailAddressUniquifier>(userAreaDefinitionCode, typeof(IEmailAddressUniquifier<>));
        return service.Uniquify(emailAddress);
    }

    //public virtual EmailAddressParts NormalizeEmailAsParts(string userAreaDefinitionCode, string emailAddress)
    //{
    //    var service = CreateServiceForUserArea<IEmailAddressNormalizer>(userAreaDefinitionCode, typeof(IEmailAddressNormalizer<>));
    //    return service.NormalizeAsParts(emailAddress);
    //}

    //public virtual EmailAddressParts NormalizeEmailAsParts(IUserAreaDefinition userAreaDefinition, string emailAddress)
    //{
    //    var service = CreateServiceForUserArea<IEmailAddressNormalizer>(userAreaDefinition, typeof(IEmailAddressNormalizer<>));
    //    return service.NormalizeAsParts(emailAddress);
    //}

    //public virtual EmailAddressParts UniquifyEmailAsParts(IUserAreaDefinition userAreaDefinition, string emailAddress)
    //{
    //    var service = CreateServiceForUserArea<IEmailAddressUniquifier>(userAreaDefinition, typeof(IEmailAddressUniquifier<>));
    //    return service.UniquifyAsParts(emailAddress);
    //}

    //public virtual EmailAddressParts UniquifyEmailAsParts(string userAreaDefinitionCode, string emailAddress)
    //{
    //    var service = CreateServiceForUserArea<IEmailAddressUniquifier>(userAreaDefinitionCode, typeof(IEmailAddressUniquifier<>));
    //    return service.UniquifyAsParts(emailAddress);
    //}

    //public virtual string NormalizeUsername(string userAreaDefinitionCode, string username)
    //{
    //    var service = CreateServiceForUserArea<IUsernameNormalizer>(userAreaDefinitionCode, typeof(IUsernameNormalizer<>));
    //    return service.Normalize(username);
    //}

    //public virtual string NormalizeUsername(IUserAreaDefinition userAreaDefinition, string username)
    //{
    //    var service = CreateServiceForUserArea<IUsernameNormalizer>(userAreaDefinition, typeof(IUsernameNormalizer<>));
    //    return service.Normalize(username);
    //}

    public virtual string? UniquifyUsername(string userAreaDefinitionCode, string? username)
    {
        var service = CreateServiceForUserArea<IUsernameUniquifier>(userAreaDefinitionCode, typeof(IUsernameUniquifier<>));
        return service.Uniquify(username);
    }

    //public virtual string UniquifyUsername(IUserAreaDefinition userAreaDefinition, string username)
    //{
    //    var service = CreateServiceForUserArea<IUsernameUniquifier>(userAreaDefinition, typeof(IUsernameUniquifier<>));
    //    return service.Uniquify(username);
    //}

    private T CreateServiceForUserArea<T>(IUserAreaDefinition userAreaDefinition, Type genericServiceType)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(userAreaDefinition);

        // Try and find a factory registered for the specific user area
        var definitionType = userAreaDefinition.GetType();
        var genericType = genericServiceType.MakeGenericType(definitionType);
        var genericInstance = _serviceProvider.GetService(genericType);

        if (genericInstance != null) return (T)genericInstance;

        return (T)_serviceProvider.GetRequiredService<T>();
    }

    private T CreateServiceForUserArea<T>(string userAreaDefinitionCode, Type genericServiceType)
        where T : notnull
    {
        var userAreaDefinition = _userAreaDefinitionRepository.GetRequiredByCode(userAreaDefinitionCode);

        return CreateServiceForUserArea<T>(userAreaDefinition, genericServiceType);
    }
}
