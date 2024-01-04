namespace Cofoundry.Domain;

/// <summary>
/// A lightweight projection of an <see cref="IUserAreaDefinition"/> 
/// implementation which contains only basic identification properties 
/// such as code and name. This is typically used as part of another 
/// aggregate projection.
/// </summary>
public class UserAreaMicroSummary
{
    /// <summary>
    /// 3 letter code identifying this user area.
    /// </summary>
    public string UserAreaCode { get; set; } = string.Empty;

    /// <summary>
    /// Display name of the area, used in the Cofoundry admin panel
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A placeholder value to use for not-nullable values that you
    /// know will be initialized in later code. This value should not
    /// be used in data post-initialization.
    /// </summary>
    public static readonly UserAreaMicroSummary Uninitialized = new();
}
