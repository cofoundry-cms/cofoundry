﻿namespace Cofoundry.Web.Admin.Internal;

public class ForgotPasswordViewModel
{
    [Required]
    [Display(Name = "Email")]
    [EmailAddress]
    public string Username { get; set; } = string.Empty;
}
