﻿using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Core.Validation;

/// <summary>
/// A validation exception that can relate to a specific property or number of properties 
/// on an object, and can optionally be represented by an error code.
/// </summary>
public class ValidationErrorException : ValidationException
{
    /// <summary>
    /// Initializes a new instance using an error message generated by the system.
    /// </summary>
    public ValidationErrorException()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance using a specified client-friendly error message.
    /// </summary>
    /// <param name="message">Client-friendly text describing the error.</param>
    public ValidationErrorException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance using a specified client-friendly error message.
    /// and an inner exception.
    /// </summary>
    /// <param name="message">Client-friendly text describing the error.</param>
    /// <param name="innerException">Inner exception collection.</param>
    public ValidationErrorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance using the data in the specified validation error
    /// instance.
    /// </summary>
    /// <param name="validationError">
    /// Validation error instance containing a message, code and any property references
    /// to copy to the exception data.
    /// </param>
    public ValidationErrorException(ValidationError validationError)
        : base(new ValidationResult(validationError.Message, validationError.Properties), null, null)
    {
        ErrorCode = validationError.ErrorCode;
    }

    /// <summary>
    /// Optional alphanumeric code representing the error that can be detected by the
    /// client to use in conditional UI flow.
    /// </summary>
    public string? ErrorCode { get; private set; }

    /// <summary>
    /// Creates a new ValidationErrorException instance that applies to one or more properties.
    /// </summary>
    /// <param name="message">Client-friendly text describing the error.</param>
    /// <param name="properties">Zero or more properties that the error message applies to.</param>
    /// <returns>New ValidationErrorException instance.</returns>
    public static ValidationErrorException CreateWithProperties(string message, params string[]? properties)
    {
        return new ValidationErrorException(new ValidationError()
        {
            Message = message,
            Properties = properties ?? []
        });
    }

    /// <summary>
    /// Creates a new ValidationErrorException instance with an error code that can
    /// be used to identify the error in a remote client application.
    /// </summary>
    /// <param name="message">Client-friendly text describing the error.</param>
    /// <param name="code">
    /// Alphanumeric code representing the error that can be detected by the
    /// client to use in conditional UI flow.
    /// </param>
    /// <returns>New ValidationErrorException instance.</returns>
    public static ValidationErrorException CreateWithCode(string message, string code)
    {
        return new ValidationErrorException(new ValidationError()
        {
            Message = message,
            ErrorCode = code
        });
    }

    /// <summary>
    /// Creates a new ValidationErrorException instance that applies to one or more properties, as
    /// well as an an error code that can be used to identify the error in a remote client application.
    /// </summary>
    /// <param name="message">Client-friendly text describing the error.</param>
    /// <param name="code">
    /// Alphanumeric code representing the error that can be detected by the
    /// client to use in conditional UI flow.
    /// </param>
    /// <param name="properties">Zero or more properties that the error message applies to.</param>
    /// <returns>New ValidationErrorException instance.</returns>
    public static ValidationErrorException CreateWithCodeAndProperties(string message, string code, params string[]? properties)
    {
        return new ValidationErrorException(new ValidationError()
        {
            Message = message,
            ErrorCode = code,
            Properties = properties ?? []
        });
    }
}
