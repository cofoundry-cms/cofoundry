using Cofoundry.Core.Mail;

namespace SendGridExample;

/// <summary>
/// Each email consists of a template class and a set of
/// template files. The template class is used as the view 
/// model and so you can include any custom propeties that
/// want to make available in the template views.
/// </summary>
public class ExampleMailTemplate : IMailTemplate
{
    /// <summary>
    /// Full path to the view file. This should not include the type part 
    /// or file extension (i.e. '_html.cshml' or '_text.cshml') because this 
    /// is automatically added.
    /// 
    /// You can place your templates wherever you like e.g. in the Cofoudry 
    /// folder as we do here, or in the Views folder.
    /// </summary>
    public string ViewFile => "~/Cofoundry/MailTemplates/ExampleMailTemplate";

    /// <summary>
    /// Used as the subject line in the email.
    /// </summary>
    public string Subject => "Example Mail";

    /// <summary>
    /// This is a custom property to hold some data that we'd like to render in our view file.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
