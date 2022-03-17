using Cofoundry.Core.Validation.Internal;
using System.Diagnostics.CodeAnalysis;

namespace Cofoundry.Core.Validation;

/// <summary>
/// Used to create templates of known errors, which can then be used
/// as a factory to create new instances with customized properties
/// or messages.
/// </summary>
public class ValidationErrorTemplate
{
    /// <summary>
    /// Creates a new <see cref="ValidationErrorTemplate"/> instance.
    /// </summary>
    /// <param name="code">
    /// The code that uniquely identifies this class of error. Canot be <see langword="null"/> 
    /// or empty. Errors codes are typically lowercase and use a dash-separated namespacing convention 
    /// e.g. "cf-my-entity-example-condition. Codes can be used in client-side code
    /// for conditional UI flow
    /// </param>
    /// <param name="message">
    /// Client-friendly text describing the error. Cannot be <see langword="null"/> or empty.
    /// </param>
    public ValidationErrorTemplate(string code, string message)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new ArgumentEmptyException(nameof(code));
        if (string.IsNullOrWhiteSpace(message)) throw new ArgumentEmptyException(nameof(message));

        ErrorCode = code;
        Message = message;
    }

    /// <summary>
    /// Creates a new <see cref="ValidationErrorTemplate"/> instance.
    /// </summary>
    /// <param name="errorCode">
    /// The code that uniquely identifies this class of error. Canot be <see langword="null"/> 
    /// or empty. Errors codes are typically lowercase and use a dash-separated namespacing convention 
    /// e.g. "cf-my-entity-example-condition. Codes can be used in client-side code
    /// for conditional UI flow
    /// </param>
    /// <param name="message">
    /// Client-friendly text describing the error. Cannot be <see langword="null"/> or empty.
    /// </param>
    public ValidationErrorTemplate(string errorCode, string message, Func<ValidationError, ValidationErrorException> exceptionFactory)
    {
        if (string.IsNullOrWhiteSpace(errorCode)) throw new ArgumentEmptyException(nameof(errorCode));
        if (string.IsNullOrWhiteSpace(message)) throw new ArgumentEmptyException(nameof(message));

        ErrorCode = errorCode;
        Message = message;
        ExceptionFactory = exceptionFactory;
    }

    /// <summary>
    /// The code that uniquely identifies this class of error. Errors codes
    /// are typically lowercase and use a dash-separated namespacing convention 
    /// e.g. "cf-my-entity-example-condition. Codes can be used in client-side code
    /// for conditional UI flow
    /// </summary>
    public string ErrorCode { get; private set; }

    /// <summary>
    /// Client-friendly text describing the error.
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// A factory function to use when throwing the error, allowing you to specify a
    /// more specific exception to throw when <see cref="Throw"/> is called. If <see langword="null"/>
    /// then <see cref="Throw"/> will throw an <see cref="ValidationErrorException"/>.
    /// </summary>
    public Func<ValidationError, ValidationErrorException> ExceptionFactory { get; set; }

    /// <summary>
    /// Creates a new <see cref="ValidationError"/> instance from the template,
    /// using the configured <see cref="ErrorCode"/> and <see cref="Message"/>.
    /// </summary>
    /// <param name="properties">
    /// Zero or more properties that the error message applies to.
    /// </param>
    /// <returns>New <see cref="ValidationError"/> instance.</returns>
    public ValidationError Create(params string[] properties)
    {
        var error = new ValidationError()
        {
            ErrorCode = ErrorCode,
            Message = Message,
            ExceptionFactory = ExceptionFactory
        };

        if (!EnumerableHelper.IsNullOrEmpty(properties))
        {
            error.Properties = properties;
        }

        return error;
    }

    /// <summary>
    /// Customize the template with instance specific configuration such as a
    /// custom message or properties.
    /// </summary>
    /// <returns>
    /// A builder instance to chain customizations to. Call <see cref="ValidationErrorTemplateErrorBuilder.Create"/> to finish building.
    /// </returns>
    public ValidationErrorTemplateErrorBuilder Customize()
    {
        return new ValidationErrorTemplateErrorBuilder(this);
    }

    /// <summary>
    /// Throws a <see cref="ValidationErrorException"/> using the configured erro <see cref="ErrorCode"/> 
    /// and <see cref="Message"/>.
    /// </summary>
    /// <param name="properties">
    /// Zero or more properties that the error message applies to.
    /// </param>
    [DoesNotReturn]
    public void Throw(params string[] properties)
    {
        var error = Create(properties);

        error.Throw();
    }
}
