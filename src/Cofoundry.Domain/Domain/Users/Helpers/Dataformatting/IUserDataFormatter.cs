namespace Cofoundry.Domain;

/// <summary>
/// Used to format email address and username properties relating to 
/// <see cref="Data.User"/> records, ensuring we use consistent formatting
/// rules whenever they are referenced in queries or updated in commands.
/// This interface is an abstration over various built-in and custom formatters, 
/// ensuring that the correct implementation is selected for a user area.
/// </summary>
public interface IUserDataFormatter
{
    /// <summary>
    /// Normalizes the specified email address into a consistent 
    /// format. The default implementation trims the input and lowercases the
    /// domain part of the email e.g. "Test@Example.com" becomes "Test@example.com". 
    /// If the email is an invalid format then <see langword="null"/> is returned.
    /// </summary>
    /// <param name="userAreaCode">
    /// The user area is required to determine if a custom <see cref="IEmailAddressNormalizer{IUserAreaDefinition}"/>
    /// implementation should be used.
    /// </param>
    /// <param name="emailAddress">
    /// The email address string to format. If the value is <see langword="null"/> 
    /// then <see langword="null"/> is returned.
    /// </param>
    string NormalizeEmail(string userAreaCode, string emailAddress);

    /// <summary>
    /// <para>
    /// Formats an email address into a value that can be used for
    /// uniqueness comparisons. The default implementation simply lowercases
    /// the email, but this can be overriden to provide more strict uniqueness
    /// checks.
    /// </para>
    /// <para>
    /// As an example, if the user types their email as " L.Balfour+cofoundry@Example.com" then by
    /// default the email would normalize via <see cref="IEmailAddressNormalizer"/> 
    /// as "L.Balfour+cofoundry@example.com" and uniquify as "l.balfour+cofoundry@example.com",
    /// however if you wanted to exclude "plus addressing" from email uniqueness checks, you could 
    /// override the default implementation with your own custom <see cref="IEmailAddressUniquifier{IUserAreaDefinition}"/>
    /// implementation. 
    /// </para>
    /// </summary>
    /// <param name="userAreaCode">
    /// The user area is required to determine if a custom <see cref="IEmailAddressUniquifier{IUserAreaDefinition}"/>
    /// implementation should be used.
    /// </param>
    /// <param name="emailAddress">
    /// The email address string to format. If the value is <see langword="null"/> 
    /// then <see langword="null"/> is returned.
    /// </param>
    string UniquifyEmail(string userAreaCode, string emailAddress);

    /// <summary>
    /// <para>
    /// Formats the specified username into a consistent format that can be
    /// used for comparing usernames e.g. as a unique value to prevent duplication
    /// and during sign in to lookup a username. The default implementation 
    /// lowercases the username. If the username is null or empty then
    /// <see langword="null"/> is returned.
    /// </para>
    /// <para>
    /// As an example, if the user types their username as " L.Balfour" then under
    /// default rules the username would normalize via <see cref="IUsernameNormalizer"/> 
    /// as "L.Balfour" and uniquify as "l.balfour". 
    /// </para>
    /// </summary>
    /// <param name="userAreaDefinitionCode">
    /// The user area is required to determine if a custom <see cref="IUsernameUniquifier{IUserAreaDefinition}"/>
    /// implementation should be used.
    /// </param>
    /// <param name="username">
    /// The username string to format. If the value is <see langword="null"/> 
    /// then <see langword="null"/> is returned.
    /// </param>
    string UniquifyUsername(string userAreaDefinitionCode, string username);

    /// <summary>
    /// Formats an email address into the various formats required to update a User record.
    /// </summary>
    /// <param name="userAreaDefinitionCode">
    /// The user area is required to determine if custom <see cref="IEmailAddressNormalizer{IUserAreaDefinition}"/>
    /// or <see cref="IEmailAddressUniquifier{IUserAreaDefinition}"/> implementations should be used.
    /// </param>
    /// <param name="emailAddress">
    /// The email address string to format. If the value is <see langword="null"/> 
    /// or otherwise invalid then <see langword="null"/> is returned.
    /// </param>
    EmailAddressFormattingResult FormatEmailAddress(IUserAreaDefinition userAreaDefinition, string emailAddress);

    /// <summary>
    /// Formats a username into the various formats required to update a User record.
    /// </summary>
    /// <param name="userAreaDefinitionCode">
    /// The user area is required to determine if custom <see cref="IUsernameNormalizer{IUserAreaDefinition}"/>
    /// or <see cref="IUsernameNormalizer{IUserAreaDefinition}"/> implementations should be used.
    /// </param>
    /// <param name="emailAddress">
    /// The result of <see cref="FormatEmailAddress"/> which can be used in a two-step formatting process
    /// if the email address is also the username. If the value is <see langword="null"/> 
    /// or otherwise invalid then <see langword="null"/> is returned.
    /// </param>
    UsernameFormattingResult FormatUsername(IUserAreaDefinition userAreaDefinition, EmailAddressFormattingResult emailAddress);

    /// <summary>
    /// Formats a username into the various formats required to update a User record.
    /// </summary>
    /// <param name="userAreaDefinitionCode">
    /// The user area is required to determine if custom <see cref="IUsernameNormalizer{IUserAreaDefinition}"/>
    /// or <see cref="IUsernameNormalizer{IUserAreaDefinition}"/> implementations should be used.
    /// </param>
    /// <param name="username">
    /// The username string to format. If the value is <see langword="null"/> 
    /// or otherwise invalid then <see langword="null"/> is returned.
    /// </param>
    UsernameFormattingResult FormatUsername(IUserAreaDefinition userAreaDefinition, string username);
}
