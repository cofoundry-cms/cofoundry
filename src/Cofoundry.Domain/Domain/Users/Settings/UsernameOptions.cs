using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Options to control the formatting and validation of usernames.
    /// Note that username character validation is ignored when 
    /// <see cref="IUserAreaDefinition.UseEmailAsUsername"/> is set to
    /// <see langword="true"/>, because the format is already validated against
    /// the configured <see cref="EmailAddressOptions"/>.
    /// </summary>
    public class UsernameOptions : IValidatableObject
    {
        public const int MAX_LENGTH_BOUNDARY = 150;

        /// <summary>
        /// <para>
        /// Allows any character in a username, effectively bypassing characters
        /// validation. Defaults to <see langword="true"/>, to ensure maximum 
        /// compatibility to the widest range of usernames when integrating with 
        /// external systems. When <see langword="true"/> any settings for 
        /// <see cref="AllowAnyLetter"/>, <see cref="AllowAnyDigit"/> and 
        /// <see cref="AdditionalAllowedCharacters"/> are ignored.
        /// </para>
        /// <para>
        /// Note that username character validation is ignored when 
        /// <see cref="IUserAreaDefinition.UseEmailAsUsername"/> is set to
        /// <see langword="true"/>, because the format is already validated against
        /// the configured <see cref="EmailAddressOptions"/>.
        /// </para>
        /// </summary>
        public bool AllowAnyCharacter { get; set; } = true;

        /// <summary>
        /// <para>
        /// Allows a username to contain any character classed as a unicode letter as 
        /// determined by <see cref="Char.IsLetter"/>. This setting is ignored when
        /// <see cref="AllowAnyCharacter"/> is set to <see langword="true"/>, which is the 
        /// default behaviour.
        /// </para>
        /// <para>
        /// Note that username character validation is ignored when 
        /// <see cref="IUserAreaDefinition.UseEmailAsUsername"/> is set to
        /// <see langword="true"/>, because the format is already validated against
        /// the configured <see cref="EmailAddressOptions"/>.
        /// </para>
        /// </summary>
        public bool AllowAnyLetter { get; set; } = true;

        /// <summary>
        /// <para>
        /// Allows a username to contain any character classed as a decimal digit as 
        /// determined by <see cref="Char.IsDigit"/> i.e 0-9. This setting is ignored when
        /// <see cref="AllowAnyCharacter"/> is set to <see langword="true"/>, which is the 
        /// default behaviour.
        /// </para>
        /// <para>
        /// Note that username character validation is ignored when 
        /// <see cref="IUserAreaDefinition.UseEmailAsUsername"/> is set to
        /// <see langword="true"/>, because the format is already validated against
        /// the configured <see cref="EmailAddressOptions"/>.
        /// </para>
        /// </summary>
        public bool AllowAnyDigit { get; set; } = true;

        /// <summary>
        /// Allows any of the specified characters in addition to the letters or digit
        /// characters permitted by the <see cref="AllowAnyLetter"/> and <see cref="AllowAnyDigit"/>
        /// settings. This setting is ignored when <see cref="AllowAnyCharacter"/> is set to
        /// <see langword="true"/>, which is the default behavior.
        /// <para>
        /// The default settings specifies a handful of special characters commonly found in
        /// usernames: "-._' ".
        /// </para>
        /// <para>
        /// Note that username character validation is ignored when 
        /// <see cref="IUserAreaDefinition.UseEmailAsUsername"/> is set to
        /// <see langword="true"/>, because the format is already validated against
        /// the configured <see cref="EmailAddressOptions"/>.
        /// </para>
        /// </summary>
        public string AdditionalAllowedCharacters { get; set; } = "-._' ";

        /// <summary>
        /// The minimum length of a username. Defaults to 1. Must be between 1 and 
        /// 150 characters.
        /// </summary>
        public int MinLength { get; set; } = 1;

        /// <summary>
        /// The maximum length of a username. Defaults to 150 characters and must be between 1 and 150 characters.
        /// </summary>
        [Range(1, MAX_LENGTH_BOUNDARY)]
        public int MaxLength { get; set; } = MAX_LENGTH_BOUNDARY;

        /// <summary>
        /// If <see langword="true"/> then the normalized username is automatically copied 
        /// to the display name field whenever it is updated. The display name field will
        /// no longer be able to be updated independently.
        /// </summary>
        public bool UseAsDisplayName { get; set; }

        /// <summary>
        /// Copies the options to a new instance, which can be modified
        /// without altering the base settings. This is used for user area
        /// specific configuration.
        /// </summary>
        public UsernameOptions Clone()
        {
            return new UsernameOptions()
            {
                AllowAnyLetter = AllowAnyLetter,
                AllowAnyDigit = AllowAnyDigit,
                AdditionalAllowedCharacters = AdditionalAllowedCharacters,
                AllowAnyCharacter = AllowAnyCharacter,
                MinLength = MinLength,
                MaxLength = MaxLength,
                UseAsDisplayName = UseAsDisplayName
            };
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MinLength > MaxLength)
            {
                yield return new ValidationResult(
                    $"{nameof(MinLength)} cannot be longer then {nameof(MaxLength)}",
                    new string[] { nameof(MinLength), nameof(MaxLength) }
                    );
            }
        }
    }
}
