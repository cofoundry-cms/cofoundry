﻿using Cofoundry.Core.Validation;

namespace Cofoundry.Core.Configuration;

/// <summary>
/// Exception to use when configuration settings are invalid for the 
/// requested operation.
/// </summary>
public class InvalidConfigurationException : Exception
{
    /// <summary>
    /// Creates a new InvalidConfigurationException with a custom message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidConfigurationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Creates a new InvalidConfigurationException relating to multiple property
    /// errors.
    /// </summary>
    /// <param name="configType">The settings type that contains the errors.</param>
    /// <param name="errors">Collection of validation errors on the settings type.</param>
    public InvalidConfigurationException(Type configType, IEnumerable<ValidationError> errors)
        : base(GetMessage(configType.Name, errors.Select(e => e.Message).FirstOrDefault()))
    {
    }

    /// <summary>
    /// Creates a new InvalidConfigurationException relating to a single error message
    /// </summary>
    /// <param name="configType">The settings type that contains the errors.</param>
    /// <param name="errorMessage">The error message to include in the exception.</param>
    public InvalidConfigurationException(Type configType, string errorMessage)
        : base(GetMessage(configType.Name, errorMessage))
    {
    }

    /// <summary>
    /// Creates a new InvalidConfigurationException relating to a single error message
    /// </summary>
    /// <param name="configName">The name of the invalid configuration setting.</param>
    /// <param name="errorMessage">The error message to include in the exception.</param>
    public InvalidConfigurationException(string configName, string errorMessage)
        : base(GetMessage(configName, errorMessage))
    {
    }

    private static string GetMessage(string configName, string? errorMessage)
    {
        errorMessage = errorMessage ?? "Unknown error.";
        return configName + " configuration invalid: " + errorMessage;
    }
}
