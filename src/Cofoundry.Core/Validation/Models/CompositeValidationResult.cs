﻿using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Core.Validation;

/// <summary>
/// A validation result that contains a group of validation results.
/// </summary>
public class CompositeValidationResult : ValidationResult
{
    private readonly List<ValidationResult> _results = [];

    public IEnumerable<ValidationResult> Results
    {
        get
        {
            return _results;
        }
    }

    public CompositeValidationResult(string errorMessage) : base(errorMessage) { }
    public CompositeValidationResult(string errorMessage, IEnumerable<string> memberNames) : base(errorMessage, memberNames) { }
    protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) { }

    public void AddResult(ValidationResult validationResult)
    {
        _results.Add(validationResult);
    }
}
