﻿namespace Cofoundry.Web.Admin;

/// <summary>
/// Dto used because json ignore on password properties prevent them
/// from being serialized.
/// </summary>
public class UpdateCurrentUserPasswordCommandDto
{
    [Required]
    public string OldPassword { get; set; } = string.Empty;

    [Required]
    [StringLength(300, MinimumLength = 8)]
    public string NewPassword { get; set; } = string.Empty;
}
