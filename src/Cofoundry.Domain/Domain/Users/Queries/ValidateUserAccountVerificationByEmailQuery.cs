﻿namespace Cofoundry.Domain;

/// <summary>
/// Determines if an email-based account verification request is valid. The result is returned as a 
/// <see cref="AuthorizedTaskTokenValidationResult"/> which describes any errors that have occurred.
/// </summary>
public class ValidateUserAccountVerificationByEmailQuery : IQuery<AuthorizedTaskTokenValidationResult>
{
    /// <summary>
    /// A token used to identify and authenticate the request. May be <see langword="null"/> 
    /// if the token was not present in the querystring.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area
    /// the user is expected to belong to.
    /// </summary>
    [Required]
    [StringLength(3)]
    public string UserAreaCode { get; set; } = string.Empty;
}
