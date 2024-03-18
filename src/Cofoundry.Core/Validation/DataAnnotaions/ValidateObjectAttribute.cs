﻿using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Core.Validation;

/// <summary>
/// Validates a child object property
/// </summary>
/// <remarks>
/// Adapted from http://www.technofattie.com/2011/10/05/recursive-validation-using-dataannotations.html
/// </remarks>
public class ValidateObjectAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        // if this a collection, validate each item.
        if (value is IEnumerable)
        {
            foreach (object collectionValue in (IEnumerable)value)
            {
                ValidateValue(collectionValue, results);
            }
        }
        else
        {
            ValidateValue(value, results);
        }

        if (results.Count != 0)
        {
            var msg = string.Format("{0} validation failed", validationContext.DisplayName);
            var memberNames = string.IsNullOrEmpty(validationContext.MemberName) ? Array.Empty<string>() : [validationContext.MemberName];
            var compositeResults = new CompositeValidationResult(msg, memberNames);

            results.ForEach(compositeResults.AddResult);

            return compositeResults;
        }

        return ValidationResult.Success;
    }

    private void ValidateValue(object? value, List<ValidationResult> results)
    {
        // ValidationContext constructor requires value to not be null.
        if (value == null) return;

        var context = new ValidationContext(value, null, null);

        Validator.TryValidateObject(value, context, results, true);
    }
}
