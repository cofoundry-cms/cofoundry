namespace Cofoundry.Core.Mail
{
    /// <summary>
    /// Simple factory for creating new <see cref="IMailDispatchSession"/>
    /// instances. The default implementation resovled the <see cref="IMailDispatchSession"/>
    /// from the DI container.
    /// </summary>
    public interface IMailDispatchSessionFactory
    {
        /// <summary>
        /// Creates a new mail session that can be used to send batches of mail.
        /// </summary>
        /// <returns>New instance of an <see cref="IMailDispatchSession"/>.</returns>
        IMailDispatchSession Create();
    }
}
