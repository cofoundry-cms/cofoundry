Cofoundry does not provide any front-end views to manage your authentication process and instead provides a flexible API to help you construct your own authentication and user management features. This gives you the freedom to fully tailor your custom user area to your requirements.

Data can be accessed via the [Content Repository](/content-management/accessing-data-programmatically) fluent APIs. `IContentRepository` includes most queries, while `IAdvancedContentRepository` is extended to include all the commands and authentication features.

## In this section:

- [Query the Current User](Query-the-Current-User): Access data for the currently signed in user.
- [Authentication](Authentication): Sign in, sign out and forcing a password change.
- [Account Registration](Account-Registration): Self-service user account registration.
- [Account Verification](Account-Verification): Updating the account verification status and using the email-based verification flow.
- [Account Recovery](Account-Recovery): Self service account recovery, also known as "forgot password" or "reset password".
- [Account Management](Account-Management): Updating the current user account, changing the password and account deletion.
- [External Auth](External-Auth): Provisioning users for external authentication mechanisms.
- [Authorized Tasks](Authorized-Tasks): A framework for authorizing single user-based operations via secure tokens e.g. account recovery or verification.

The examples in this section use the bare .NET APIs and are not specific to MVC, Razor Rages or web APIs, however you can refer to the [sample projects](/user-areas/samples) for more specific examples using these technologies. Most of the APIs require the `IAdvancedContentRepository` which can be injected via DI, but this is also omitted from the examples.
