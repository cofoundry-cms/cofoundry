namespace Cofoundry.Domain;

public class GetSettingsQuery<TEntity>
    : IQuery<TEntity>
    where TEntity : ICofoundrySettings
{
}
