using System.Diagnostics.CodeAnalysis;

namespace Cofoundry.Core.Validation.Internal;

/// <summary>
/// Used to customize a <see cref="ValidationErrorTemplate"/> with instance
/// specific configuration such as custom properties.
/// </summary>
public class ValidationErrorTemplateErrorBuilder
{
    private readonly ValidationError _validationError;

    public ValidationErrorTemplateErrorBuilder(
        ValidationErrorTemplate validationErrorTemplate
        )
    {
        ArgumentNullException.ThrowIfNull(validationErrorTemplate);

        _validationError = validationErrorTemplate.Create();
    }

    /// <summary>
    /// Adds the specified <paramref name="errorCodeSuffix"/> to the end
    /// of the existing error code, separated by a '-' character.
    /// </summary>
    /// <param name="errorCodeSuffix">The text to add to the end of the existing error code.</param>
    public ValidationErrorTemplateErrorBuilder WithErrorCodeSuffix(string errorCodeSuffix)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(errorCodeSuffix);

        _validationError.ErrorCode = _validationError.ErrorCode + "-" + errorCodeSuffix.TrimStart('-');
        return this;
    }

    /// <summary>
    /// Replaces the existing template error message with a custom one.
    /// </summary>
    /// <param name="message">
    /// Client-friendly text describing the error. Cannot be <see langword="null"/> or empty.
    /// </param>
    public ValidationErrorTemplateErrorBuilder WithMessage(string message)
    {
        ArgumentEmptyException.ThrowIfNullOrWhitespace(message);

        _validationError.Message = message;
        return this;
    }

    /// <summary>
    /// Formats the existing error message with the specified <paramref name="messageFormatParameters"/> via
    /// <see cref="string.Format"/>.
    /// </summary>
    /// <param name="messageFormatParameters">
    /// The parameters to pass into <see cref="string.Format"/> to format the error emssage.
    /// </param>
    public ValidationErrorTemplateErrorBuilder WithMessageFormatParameters(params string[] messageFormatParameters)
    {
        if (EnumerableHelper.IsNullOrEmpty(messageFormatParameters)) throw new ArgumentEmptyException(nameof(messageFormatParameters));

        var message = string.Format(_validationError.Message, messageFormatParameters);
        return WithMessage(message);
    }

    /// <summary>
    /// Formats the existing error message with the specified <paramref name="messageFormatParameters"/> via
    /// <see cref="string.Format"/>.
    /// </summary>
    /// <param name="messageFormatParameters">
    /// The parameters to pass into <see cref="string.Format"/> to format the error emssage.
    /// </param>
    public ValidationErrorTemplateErrorBuilder WithMessageFormatParameters(params int[] messageFormatParameters)
    {
        if (EnumerableHelper.IsNullOrEmpty(messageFormatParameters)) throw new ArgumentEmptyException(nameof(messageFormatParameters));

        var parameters = messageFormatParameters.Select(p => p.ToString()).ToArray();
        var message = string.Format(_validationError.Message, parameters);
        return WithMessage(message);
    }

    /// <summary>
    /// Adds a <see cref="ValidationError.ExceptionFactory"/> to the template, allowing you to specify
    /// exactly which error should be thrown when a call is made to <see cref="ValidationError.Throw"/>.
    /// </summary>
    /// <param name="exceptionFactory">
    /// A factory function to use when throwing the error, allowing you to specify a
    /// more specific exception to throw when <see cref="ValidationError.Throw"/> is called.
    /// </param>
    public ValidationErrorTemplateErrorBuilder WithExceptionFactory(Func<ValidationError, ValidationErrorException> exceptionFactory)
    {
        ArgumentNullException.ThrowIfNull(exceptionFactory);

        _validationError.ExceptionFactory = exceptionFactory;
        return this;
    }

    /// <summary>
    /// Adds the specified <paramref name="propertyNames"/> to the template.
    /// </summary>
    /// <param name="propertyNames">
    /// Zero or more properties that the error message applies to.
    /// </param>
    public ValidationErrorTemplateErrorBuilder WithProperties(params string[] propertyNames)
    {
        if (EnumerableHelper.IsNullOrEmpty(propertyNames)) throw new ArgumentEmptyException(nameof(propertyNames));
        _validationError.Properties = propertyNames;

        return this;
    }

    /// <summary>
    /// Creates a new <see cref="ValidationError"/> instance from the template
    /// builder configuration.
    /// </summary>
    /// <returns>New <see cref="ValidationError"/> instance.</returns>
    public ValidationError Create()
    {
        return new ValidationError()
        {
            ErrorCode = _validationError.ErrorCode,
            ExceptionFactory = _validationError.ExceptionFactory,
            Message = _validationError.Message,
            Properties = _validationError.Properties
        };
    }

    /// <summary>
    /// Throws a <see cref="ValidationErrorException"/> using the template
    /// builder configuration.
    /// </summary>
    /// <param name="properties">
    /// Zero or more properties that the error message applies to.
    /// </param>
    [DoesNotReturn]
    public void Throw()
    {
        _validationError.Throw();
    }
}
