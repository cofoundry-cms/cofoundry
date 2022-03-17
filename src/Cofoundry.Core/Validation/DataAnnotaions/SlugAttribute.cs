using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Core.Validation;

/// <summary>
/// Validates that a field only contains characters you'd want to
/// see in a url slug i.e. AlphaNumeric and dashes. This does not enforce case
/// and any value should be converted to lowercase before persiting.
/// </summary>
public class SlugAttribute : RegularExpressionAttribute
{
    /// <summary>
    /// Validates that a field only contains characters you'd want to
    /// see in a url slug i.e. AlphaNumeric and dashes. This does not enforce case
    /// and any value should be converted to lowercase before persiting.
    /// </summary>
    public SlugAttribute() :
        base("^[a-zA-Z0-9-]+$")
    {
        ErrorMessage = "{0} can only use letters, numbers and hyphens.";
    }
}
