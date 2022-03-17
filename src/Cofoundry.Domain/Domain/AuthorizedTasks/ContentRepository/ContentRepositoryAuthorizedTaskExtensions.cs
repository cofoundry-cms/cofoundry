using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

public static class ContentRepositoryAuthorizedTaskExtensions
{
    /// <summary>
    /// <para>
    /// Authorized tasks represent a single user-based operation that can be executed without
    /// being logged in. Task authorization is validated by a crytographically random 
    /// generated token, often communicated via an out-of-band communication mechanism
    /// such as an email. Examples include password reset or email address validation flows.
    /// </para>
    /// <para>
    /// Tasks tend to be single-use and can be marked when completed, and can also be 
    /// invalidated explicitly. They can also be rate-limited by IPAddress and time-limited
    /// by validating against the <see cref="CreateDate"/>.
    /// </para>
    /// </summary>
    public static IAdvancedContentRepositoryAuthorizedTaskRepository AuthorizedTasks(this IAdvancedContentRepository contentRepository)
    {
        return new ContentRepositoryAuthorizedTaskRepository(contentRepository.AsExtendableContentRepository());
    }
}
