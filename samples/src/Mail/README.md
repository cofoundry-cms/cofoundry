# Mail Sample

This sample shows a couple of examples of how you can use the Cofoundry mail abstraction to send emails, including:

- Creating custom mail templates
- Sending mail with `IMailService`
- Customizing email notifications for admin panel accounts

For more information on using email in Cofoundry, [see the mail docs](https://www.cofoundry.org/docs/framework/mail).

## To get started:

1. Start up the local-env docker compose file to get the database running. See [local-env docs](../../local-env/README.md) for details.
2. Run the website and navigate to "/admin", which will display the setup screen
3. Enter an application name and setup your user account. Submit the form to complete the site setup. 

## Configuring mail

The sample use the default Cofoundry mail implementation, which simply writes out the rendered mail content to text files in the "App_Data/Email" folder.

This is good for testing, but for production you'll want to install a mail plugin that is compatible with your mail host. 

Refer to the [mail docs](https://www.cofoundry.org/docs/framework/mail) for more details.

##  Examples

### Sending a mail using `IMailService`

The `HomeController` shows how to build a simple email notification from a contact request form using a custom template. The template files can be found in the "Cofoundry/MailTemplates/ContactRequest" folder.

### Customizing Admin Notifications

The `AdminMailTemplateBuilder` class and the templates in the "Cofoundry/MailTemplates/Admin" folder show several different ways you can customize the admin email notifications.



