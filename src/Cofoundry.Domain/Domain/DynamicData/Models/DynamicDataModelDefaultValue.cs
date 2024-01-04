namespace Cofoundry.Domain;

/// <summary>
/// A wrapper object for a default dynamic data model instance.
/// The wrapper is required to allow a custom JSON converter to
/// run on the instance, which removed default property values.
/// </summary>
public class DynamicDataModelDefaultValue
{
    /// <summary>
    /// A new instance of the model with any default values.
    /// </summary>
    public required object Value { get; set; }

    /// <summary>
    /// A placeholder value to use for not-nullable values that you
    /// know will be initialized in later code. This value should not
    /// be used in data post-initialization.
    /// </summary>
    public static readonly DynamicDataModelDefaultValue Uninitialized = new()
    {
        Value = string.Empty
    };
}
