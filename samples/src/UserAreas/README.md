# User Areas Sample

The user areas sample solution contains two projects:

- AuthenticationSample: A standard credential-based authentication flow with account recovery and account management features.
- RegistrationAndVerificationSample: A self-service user registration and account verification flow.

## AuthenticationSample

The authentication sample is written using ASP.NET Razor Pages and implements the following features:

- Credential-based authentication (email addresses as usernames)
- Sign in/sign out
- Forcing a password change at first sign in
- Account recovery (AKA "forgot password")
- Custom mail templates
- Account management (update, password change, delete)
- Using `ICofoundryHelper` in views to query the current user

In this sample users are added via the admin panel.

### To get started:

1. Start up the local-env docker compose file to get the database running. See [local-env docs](../../local-env/README.md) for details.
2. Run the website and navigate to *"/admin"*, which will display the setup screen
3. Enter an application name and setup your user account. Submit the form to complete the site setup. 

### Adding a User

1. Sign into the admin panel
2. Navigate to **Settings > Member Users** to create a user
3. Adding a user will send an email notification to the user with a temporary password. 
4. The test site is configured to write out debug emails into the `/App_Data/Emails` folder, open the email file to find the temporary password for your user account.
5. Follow the link on the homepage or navigate to "/members/" to sign in.

## RegistrationAndVerificationSample

The registration and verification sample is written using ASP.NET MVC and implements the following features:

- Self-service user account registration
- User accounts verified by email
- Re-sending verification emails
- Custom user verification mail template
- Credential-based authentication (separate username and email address)
- Sign in/sign out
- Using `ICofoundryHelper` in views to query the current user

### To get started:

1. Start up the local-env docker compose file to get the database running. See [local-env docs](../../local-env/README.md) for details.
2. Run the website and navigate to *"/admin"*, which will display the setup screen
3. Enter an application name and setup your user account. Submit the form to complete the site setup. 

### Registering a User

1. From the homepage navigate to **Members > Register here** or go directly to "/members/registration".
2. Fill in the and submit the form.
3. Adding a user will send an email notification to the user with a verification link. 
4. The test site is configured to write out debug emails into the `/App_Data/Emails` folder, open the email file to find the verification link.
5. Copy the link into your browser, navigating to the link will verify the account.
6. Follow the "sign in" link to sign in to your new account.

