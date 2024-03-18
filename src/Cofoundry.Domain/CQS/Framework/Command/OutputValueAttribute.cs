﻿using System.Reflection;

namespace Cofoundry.Domain.CQS;

/// <summary>
/// Used for an output property of an ICommand. This validates that the
/// property has not been assigned to prior to execution.
/// </summary>
public class OutputValueAttribute : ValidationAttribute
{
    public OutputValueAttribute() :
        base("Output value must not be assigned.")
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;

        var type = value.GetType();

        if (type.GetTypeInfo().IsValueType && value.Equals(Activator.CreateInstance(type)))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(base.ErrorMessage);
    }
}
