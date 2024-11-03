## Complete Sample Sites

The complete sample sites are the best way to get a feel for how a Cofoundry site works and what the code looks like. 

### [Cofoundry.Samples.SimpleSite](https://github.com/cofoundry-cms/Cofoundry.Samples.SimpleSite)

A simple website implementing content management and some framework features including:

- Startup registration
- Page templates
- Custom page block types
- Image content
- Two custom entities - blog post and category
- Querying and display a list of blog posts
- A blog post custom entity details page
- A simple contact form
- Email notifications & email templating
- Custom error pages
- Configuration settings

[View on GitHub](https://github.com/cofoundry-cms/Cofoundry.Samples.SimpleSite)


### [Cofoundry.Samples.SPASite](https://github.com/cofoundry-cms/Cofoundry.Samples.SPASite)

An example demonstrating how to use Cofoundry to create a SPA (Single Page Application) with WebApi endpoints as well as demonstrating some advanced Cofoundry features.

The application is also separated into two projects demonstrating an example of how you might structure your domain layer to keep this layer separate from your web layer.

- Startup registration
- Web API and use of `IApiResponseHelper`
- Structuring commands and queries using CQS 
- Multiple related custom entities - Cats, Breeds and Features
- A member area with a registration and sign in process
- Using an entity framework DbContext to represent custom database tables
- Executing stored procedures using `IEntityFrameworkSqlExecutor`
- Integrating custom entity data with entity framework data access
- Using the auto-updater to run SQL scripts
- Email notifications & email templating
- Registering services with the DI container

[View on GitHub](https://github.com/cofoundry-cms/Cofoundry.Samples.SPASite)

## Individual Samples

These samples focus on specific areas of functionality in Cofoundry.

### [Cofoundry.Samples.Mail](https://github.com/cofoundry-cms/Cofoundry.Samples.Mail)

This sample shows a couple of examples of how you can use the Cofoundry mail abstraction to send emails, including:

- Creating custom mail templates
- Sending mail with `IMailService`
- Customizing email notifications for admin panel accounts

[View on GitHub](https://github.com/cofoundry-cms/Cofoundry.Samples.Mail)

### [Cofoundry.Samples.Menus](https://github.com/cofoundry-cms/Cofoundry.Samples.Menus)

A bare website showing various examples of creating content editable menus:

- Simple Menu
- Nested Menu
- Multi-level Menu

[View on GitHub](https://github.com/cofoundry-cms/Cofoundry.Samples.Menus)

### [Cofoundry.Samples.PageBlockTypes](https://github.com/cofoundry-cms/Cofoundry.Samples.PageBlockTypes)

A bare website showing various examples of how to implement page block types including

- Using data model attributes
- Querying for data
- Managing related entities and versions

[View on GitHub](https://github.com/cofoundry-cms/Cofoundry.Samples.PageBlockTypes)

### [Cofoundry.Samples.UserAreas](https://github.com/cofoundry-cms/Cofoundry.Samples.UserAreas)

The user areas sample solution contains two projects:

- AuthenticationSample: A standard credential-based authentication flow with account recovery and account management features.
- RegistrationAndVerificationSample: A self-service user registration and account verification flow.

#### AuthenticationSample

This sample is written using ASP.NET Razor Pages and implements the following features:

- Credential-based authentication (email addresses as usernames)
- Sign in/sign out
- Forcing a password change at first sign in
- Account recovery (AKA "forgot password")
- Custom mail templates
- Account management (update, password change, delete)
- Using `ICofoundryHelper` in views to query the current user

In this sample users are added via the admin panel.

#### RegistrationAndVerificationSample

This sample is written using ASP.NET MVC and implements the following features:

- Self-service user account registration
- User accounts verified by email
- Re-sending verification emails
- Custom user verification mail template
- Credential-based authentication (separate username and email address)
- Sign in/sign out
- Using `ICofoundryHelper` in views to query the current user

[View on GitHub](https://github.com/cofoundry-cms/Cofoundry.Samples.UserAreas)
