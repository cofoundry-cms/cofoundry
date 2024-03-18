﻿using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Core.Validation;

/// <summary>
/// Validates the property is a positive integer value. Useful
/// for making sure an Id/Key field has been entered.
/// </summary>
public class PositiveIntegerAttribute : RangeAttribute
{
    public PositiveIntegerAttribute() :
        base(1, int.MaxValue)
    {
        ErrorMessage = "The {0} field must be a positive integer value.";
    }
}
