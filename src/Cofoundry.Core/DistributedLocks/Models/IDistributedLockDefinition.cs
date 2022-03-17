namespace Cofoundry.Core.DistributedLocks;

/// <summary>
/// Defines a distributed lock type whereby only a single
/// lock can be held on this type across multiple instances
/// of the same application. For example the auto-update system
/// uses distributed locks to ensure that only one application
/// instance can run updates at any one time. 
/// </summary>
public interface IDistributedLockDefinition
{
    /// <summary>
    /// A unique identifier for the lock. Must be alphanumeric and
    /// be 6 characters e.g. "COFAUT". The uniqueness comparisson is
    /// case-insensitive.
    /// </summary>
    string DistributedLockId { get; }

    /// <summary>
    /// A display friendly name for the lock e.g. "Auto update".
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The duration to wait before the lock expires and allowing
    /// other processes to take a new lock. This is required because
    /// a fault could occur which prevents the lock from closing e.g.
    /// an unexpected app shutdown.
    /// </summary>
    TimeSpan Timeout { get; }
}
