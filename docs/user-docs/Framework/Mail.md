> This feature is demonstrated in the [Cofoundry.Samples.Mail](https://github.com/cofoundry-cms/Cofoundry.Samples.Mail) sample project.

Cofoundry includes a mail service abstraction that makes it easy to create and send email from anywhere in your application.

The default implementation simply writes out mail to text files to a directory for debugging purposes, but you can use plugins to change this behavior and scale your mail solution as your application demands it.

Currently Available Plugins:

- **[Cofoundry.Plugins.Mail.MailKit](https://github.com/cofoundry-cms/Cofoundry.Plugins.Mail.MailKit):** Dispatch mail using MailKit, a cross platform alternative to `System.Net.Mail`.
- **[Cofoundry.Plugins.Mail.SendGrid](https://github.com/cofoundry-cms/Cofoundry.Plugins.Mail.SendGrid):** Dispatch mail using the popular SendGrid service.

## Mail Templates

In order to send an email you first need to create a mail template. A mail template comprises of a .NET class implementing `IMailTemplate` and either an HTML view file or a plain text view file, or both.

Although most email recipients will be able to view HTML emails, it may be blocked or unreadable by some. A plain text email will have the best change of being delivered and read.

If you create both an HTML and plain text view file then a multi-part email will be sent, where the plain text version is only used as a fallback when the HTML version is not permitted. Using both is good practice and will ensure your email can be received and read by all recipients.

Here's an example of a multi-part email:

**ExampleNotificationMailTemplate.cs**

```csharp
using Cofoundry.Core.Mail;

public class ExampleNotificationMailTemplate : IMailTemplate
{
    /// <summary>
    /// We need to specify the path and name of the mail template view
    /// file. The convention is to exclude the part of the file name 
    /// that indicates the template type and the file extension
    /// </summary>
    public string ViewFile => "~/Views/EmailTemplates/ExampleNotificationMailTemplate";
     
    /// <summary>
    /// All templates require a subject
    /// </summary>
    public string Subject => "New Contact Request";
     
    /// <summary>
    /// We can include any additional data that we
    /// want to render in the template file as properties
    /// in this class
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
```

**ExampleNotificationMailTemplate_html.cshtml**

Note that the html template file should have the **'_html'** postfix

```html
@model ExampleNotificationMailTemplate

@{
    Layout = "/Views/EmailTemplates/OptionalLayoutFile_html.cshtml";
}

<h2>Hi there</h2>

<p>
    @Model.Message 
</p>

<p>
    Thanks,<br />
    Cofoundry
</p>
```

**ExampleNotificationMailTemplate_text.cshtml**

Note that the plain text template file should have the **'_text'** postfix

```csharp
@model ExampleNotificationMailTemplate

@{
    Layout = "/Views/EmailTemplates/OptionalLayoutFile_Text.cshtml";
}

Hi there

@Model.Message 

Thanks,
Cofoundry
```

### Template Rendering

Templates are rendered using razor, and is done outside of the http request scope using a faked `ViewContext`. This provides the important benefit of being able to render razor templates outside of a web request, e.g. in a background task or external service. Most razor features should work, however be aware that anything that relies on an http request will fail.

You can customize the mail rending process by implementing `IMailViewRenderer` and overriding the base implementation using the [DI system](Dependency-Injection).

### File Placement

Where you place your template files is up to you, but here's two suggested scenarios:

**Simple**

Place your `IMailTemplate` code files with your models or domain and place your template files in your views directory. For an example of this see  [Cofoundry.Samples.SimpleSite](https://github.com/cofoundry-cms/Cofoundry.Samples.SimpleSite).

Or if you preferred to keep your files together, you could place them next to each other, as shown in the [Cofoundry.Samples.Mail](https://github.com/cofoundry-cms/Cofoundry.Samples.Mail) sample.

**In a separate project**

If you have a separate project for your domain or want to keep all your template classes and views together in one place you can do so, but to keep views in a separate assembly you'll need to mark them as embedded resources and you'll need to tell Cofoundry that you have embedded resources in your application by including a class that implements `IAssemblyResourceRegistration`. [Cofoundry itself](https://github.com/cofoundry-cms/cofoundry/tree/master/src/Cofoundry.Domain) is a good example of this.

## Sending Mail

Sending mail is pretty straightforward. Simply new up an instance of your template, populate the data and use `IMailService` to send it:

```csharp
using Cofoundry.Core.Mail;

public class MailExample
{
    private readonly IMailService _mailService;

    public MailExample(IMailService mailService)
    {
        _mailService = mailService;
    }

    public Task SendMail()
    {
        var template = new ExampleNotificationMailTemplate
        {
            Message = "Wibble, wibble"
        };

        return _mailService.SendAsync("hq@example.com", template);
    }
}
```

Note that by default *SendMode* setting is set to *LocalDrop* to prevent accidentally sending out emails when debugging. In a production scenario you'll want to change the setting to *Send*. I.e. in your production config:

```js
{
  "Cofoundry": {
    "Mail": {
      "SendMode": "Send"
    }
  }
}
```

#### From Addresses

You can set the default from address using the **Cofoundry:Mail:DefaultFromAddress** setting in your config file.

Alternatively you can inherit from `IMailTemplateWithCustomFromAddress` in your template file to specify a custom from address rather than using the site-wide default.

## MailSettings

The following mail settings are available:

- **Cofoundry:Mail:SendMode** Indicates whether emails should be sent and how. Uses the `MailSendMode` enum (LocalDrop, Send, SendToDebugAddress, DoNotSend)
- **Cofoundry:Mail:DebugEmailAddress** An email address to redirect all mail to when using MailSendMode.SendToDebugAddress
- **Cofoundry:Mail:DefaultFromAddress** The default address to send emails
- **Cofoundry:Mail:DefaultFromAddressDisplayName** Optionally the name to display with the default From Address
- **Cofoundry:Mail:MailDropDirectory** The path to the folder to save mail to when using SendMode.LocalDrop. Defaults to ~/App_Data/Emails



