using System.Diagnostics;
using System.Text;
using Cofoundry.Core;
using Cofoundry.Core.Mail;
using Cofoundry.Plugins.ErrorLogging.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Cofoundry.Plugins.ErrorLogging.Domain;

public class ErrorLoggingService : IErrorLoggingService
{
    private readonly IUserContextService _userContextService;
    private readonly IMailDispatchService _mailService;
    private readonly ErrorLoggingDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ErrorLoggingSettings _errorLoggingSettings;
    private readonly ILogger<ErrorLoggingService> _logger;

    public ErrorLoggingService(
        IUserContextService userContextService,
        IMailDispatchService mailService,
        ErrorLoggingDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ErrorLoggingSettings errorLoggingSettings,
        ILogger<ErrorLoggingService> logger
        )
    {
        _userContextService = userContextService;
        _mailService = mailService;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _errorLoggingSettings = errorLoggingSettings;
        _logger = logger;
    }

    public async Task LogAsync(Exception ex)
    {
        var inner = ex.GetBaseException() ?? ex;
        var httpContext = _httpContextAccessor.HttpContext;

        var error = MapError(inner, httpContext);
        string additionalText;
        try
        {
            _dbContext.Errors.Add(error);
            _dbContext.SaveChanges();
            additionalText = "\r\n\r\nSaved in DB: Yes";
        }
        catch (Exception loggingEx)
        {
            additionalText = $"\r\n\r\nSaved in DB: No\r\n\r\nException:{loggingEx.Message}";

            const string msg = "An exception occured while logging an error to the database.";
            _logger.LogError(0, loggingEx, msg);
            Debug.Assert(false, msg);
        }

        if (!string.IsNullOrWhiteSpace(_errorLoggingSettings.LogToEmailAddress))
        {
            try
            {
                await SendMailAsync(_errorLoggingSettings.LogToEmailAddress, httpContext, error, additionalText);
                error.EmailSent = true;
                _dbContext.SaveChanges();
            }
            catch (Exception mailException)
            {
                const string msg = "An exception occured while sending an error logging email.";
                _logger.LogError(0, mailException, msg);
                Debug.Assert(false, msg);
            }
        }
    }

    private static Error MapError(Exception ex, HttpContext? context)
    {
        var error = new Error
        {
            Source = TextFormatter.Limit(ex.Source, 100) ?? string.Empty,
            ExceptionType = ex.Message ?? string.Empty,
            Target = TextFormatter.Limit(ex.TargetSite?.Name, 255) ?? string.Empty,
            StackTrace = ex.StackTrace ?? string.Empty,
            EmailSent = false,
            CreateDate = DateTime.UtcNow
        };

        if (context != null && context.Request != null)
        {
            error.Url = TextFormatter.Limit(context.Request.Method + " " + context.Request.GetDisplayUrl(), 255);
            error.QueryString = TextFormatter.Limit(context.Request.QueryString.ToString(), 255);
            if (context.Request.HasFormContentType)
            {
                var formItems = context
                    .Request
                    .Form
                    .Keys
                    .Select(k => $"{k}: {context.Request.Form[k]}");
                error.Form = string.Join(Environment.NewLine, formItems);
            }
            error.UserAgent = TextFormatter.Limit(context.Request.Headers[HeaderNames.UserAgent], 255);

            if (context.Features.Get<ISessionFeature>() != null && context.Session.IsAvailable)
            {
                var sessionItems = context
                    .Session
                    .Keys
                    .Select(k => $"{k}: {context.Session.GetString(k)}");

                error.Session = string.Join(Environment.NewLine, sessionItems);
            }
        }

        if (ex.Data != null && ex.Data.Count > 0)
        {
            error.Data = string.Join(", ", ex.Data);
        }

        return error;
    }

    private async Task SendMailAsync(
        string logToEmailAddress,
        HttpContext? httpContxt,
        Error error,
        string additionalText
        )
    {
        var hasRequest = httpContxt != null && httpContxt.Request != null;
        var user = await _userContextService.GetCurrentContextAsync();
        var userText = user == null ? "Unauthenticated" : $"{user.UserArea?.Name}: {user.UserId}";

        var bodyText = new StringBuilder();
        bodyText.AppendLine("Url: " + error.Url);
        bodyText.AppendLine();
        bodyText.AppendLine("User: " + userText);
        bodyText.AppendLine();
        bodyText.AppendLine("Exception: " + error.ExceptionType);
        bodyText.AppendLine();
        bodyText.AppendLine("Stack trace:");
        bodyText.AppendLine(error.StackTrace);
        bodyText.AppendLine();

        if (hasRequest)
        {
            bodyText.AppendLine("Query string: " + error.QueryString);
            bodyText.AppendLine();

            if (!string.IsNullOrEmpty(error.UserAgent))
            {
                bodyText.AppendLine("UserAgent: " + error.UserAgent);
                bodyText.AppendLine();
            }

            if (!string.IsNullOrEmpty(error.Session))
            {
                bodyText.AppendLine("Session:");
                bodyText.AppendLine(error.Session);
                bodyText.AppendLine();
            }

            if (!string.IsNullOrWhiteSpace(error.Form))
            {
                bodyText.AppendLine("Form:");
                bodyText.AppendLine(error.Form);
                bodyText.AppendLine();
            }
        }

        if (error.Data != null)
        {
            bodyText.AppendLine("Data:");
            bodyText.AppendLine(error.UserAgent);
            bodyText.AppendLine();
        }

        bodyText.AppendLine(additionalText);
        var msg = new MailMessage
        {
            TextBody = bodyText.ToString(),
            To = new MailAddress(logToEmailAddress),
            Subject = "Exception: " + error.Source
        };

        if (!Debugger.IsAttached)
        {
            await _mailService.DispatchAsync(msg);
        }
    }
}
