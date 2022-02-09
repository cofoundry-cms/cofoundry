using System;

namespace Cofoundry.Domain.Extendable
{
    /// <summary>
    /// Functionality to be used for extension only e.g. internally 
    /// by Cofoundry, in plugins or custom extensions. Access members by referencing the
    /// <see cref="Cofoundry.Domain.Extendable"/> namespace and invoking the 
    /// <see cref="IExtendableContentRepositoryExtensions.AsExtendableContentRepository"/>
    /// on your repository instance.
    /// </summary>
    public interface IExtendableContentRepository : IAdvancedContentRepository
    {
        /// <summary>
        /// Service provider instance to be used for extension only e.g. internally 
        /// by Cofoundry, plugins or custom extensions.
        /// </summary>
        IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Clones the repository, replacing the internal <see cref="IDomainRepositoryExecutor"/>
        /// with one specified by executing the <paramref name="domainRepositoryExecutorFactory"/>
        /// function.
        /// </summary>
        /// <param name="domainRepositoryExecutorFactory">
        /// A factory for creating the new <see cref="IDomainRepositoryExecutor"/> instance, which
        /// will be invoked immediately. The factory function exposes the existing executor so
        /// that it can be wrapped (decorated) by the new executor to preserve any chained
        /// functionality.
        /// </param>
        /// <returns>
        /// The returned repository is a cloned version which should be of the same type. It
        /// is therefore safe to cast it back to the original type.
        /// </returns>
        IExtendableContentRepository WithExecutor(Func<IDomainRepositoryExecutor, IDomainRepositoryExecutor> domainRepositoryExecutorFactory);
    }
}