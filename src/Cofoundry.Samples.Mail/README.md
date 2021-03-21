# Cofoundry.Samples.Mail

This sample shows a couple of examples of how you can use the Cofoundry mail abstraction to send emails.

For more information on using email in Cofoundry, [see the mail docs](https://www.cofoundry.org/docs/framework/mail).

## To get started:

1. Create a database named 'Cofoundry.Samples.Mail' and check the Cofoundry connection string in the config file is correct for you SqlServer instance
2. Run the website and navigate to "/admin", which will display the setup screen
3. Enter an application name and setup your user account. Submit the form to complete the site setup. 

## Configuring mail

The sample use the default Cofoundry mail implementation, which simply writes out the rendered mail content to text files in the "App_Data/Email" folder.

This is good for testing, but for production you'll want to install a mail plugin that is compatible with your mail host. 

Refer to the [mail docs](https://www.cofoundry.org/docs/framework/mail) for more details.

##  Examples

#### Sending a mail using `IMailService`

The `HomeController` shows how to build a simple email notification from a contact request form using a custom template.

The template files can be found in the "Cofoundry/MailTemplates" folder.

#### Customizing Admin Notifications

The `AdminMailTemplateBuilder` class and the templates in the "Cofoundry/Admin/MailTemplates" folder show several different ways you can customize the admin email notifications.



