﻿using System.ComponentModel.DataAnnotations;

namespace Cofoundry.BasicTestSite;

public class ContactRequest
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email Address")]
    public string EmailAddress { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;
}
