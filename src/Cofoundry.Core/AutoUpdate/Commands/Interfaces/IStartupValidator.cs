namespace Cofoundry.Core.AutoUpdate.Internal;

/// <summary>
/// Used to validate some part of the system at startup
/// prior to runing auto-update commands e.g. duplicate 
/// definitions or invalid configuration settings. This is currently internal as I expect these
/// will be able to be replaced with source generators, or if
/// not, it probably needs more design thought.
/// </summary>
public interface IStartupValidator
{
    /// <summary>
    /// Run validation, throwing exceptions for any
    /// invalid state.
    /// </summary>
    void Validate();
}
