## [Cofoundry.Samples.UserAreas](https://github.com/cofoundry-cms/Cofoundry.Samples.UserAreas)

The user areas sample solution contains two projects:

- [AuthenticationSample](#AuthenticationSample): A standard credential-based authentication flow with account recovery and account management features.
- [RegistrationAndVerificationSample](#RegistrationAndVerificationSample): A self-service user registration and account verification flow.

### AuthenticationSample

This sample is written using ASP.NET Razor Pages and implements the following features:

- Credential-based authentication (email addresses as usernames)
- Sign in/sign out
- Forcing a password change at first sign in
- Account recovery (AKA "forgot password")
- Custom mail templates
- Account management (update, password change, delete)
- Using `ICofoundryHelper` in views to query the current user

In this sample users are added via the admin panel.

[View on GitHub](https://github.com/cofoundry-cms/Cofoundry.Samples.UserAreas)

### RegistrationAndVerificationSample

This sample is written using ASP.NET MVC and implements the following features:

- Self-service user account registration
- User accounts verified by email
- Re-sending of verification emails
- Custom user verification mail template
- Credential-based authentication (separate username and email address)
- Sign in/sign out
- Using `ICofoundryHelper` in views to query the current user

[View on GitHub](https://github.com/cofoundry-cms/Cofoundry.Samples.UserAreas)

## [Cofoundry.Samples.SPASite](https://github.com/cofoundry-cms/Cofoundry.Samples.SPASite)

As part of a larger sample application, this sample contains:

- A custom "Member" user area
- An API-driven interface
- A member registration form
- A member sign in form

[View on GitHub](https://github.com/cofoundry-cms/Cofoundry.Samples.SPASite)

