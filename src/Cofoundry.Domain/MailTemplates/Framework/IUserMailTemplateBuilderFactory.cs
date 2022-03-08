namespace Cofoundry.Domain.MailTemplates.Internal
{
    /// <summary>
    /// Creates the configured <see cref="IUserMailTemplateBuilder"/> implementation 
    /// that matches the specified user area, searching for any custom implementations
    /// registered in the DI system, and falling back to the default implementation.
    /// </summary>
    public interface IUserMailTemplateBuilderFactory
    {
        /// <summary>
        /// Creates the configured <see cref="IUserMailTemplateBuilder"/> implementation 
        /// that matches the specified user area, searching for any custom implementations
        /// registered in the DI system, and falling back to the default implementation.
        /// </summary>
        /// <param name="userAreaDefinitionCode">
        /// The <see cref="IUserAreaDefinition.UserAreaCode"/> of the user area the builder
        /// should create mail templates for.
        /// </param>
        IUserMailTemplateBuilder Create(string userAreaDefinitionCode);
    }
}