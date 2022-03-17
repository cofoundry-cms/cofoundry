using Cofoundry.Domain.Extendable;
using Microsoft.Extensions.DependencyInjection;

namespace Cofoundry.Domain.Internal;

public class ContentRepositoryUserRepository
        : IContentRepositoryUserRepository
        , IExtendableContentRepositoryPart
{
    public ContentRepositoryUserRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IContentRepositoryUserByIdQueryBuilder GetById(int userId)
    {
        return new ContentRepositoryUserByIdQueryBuilder(ExtendableContentRepository, userId);
    }

    public IContentRepositoryUserByEmailQueryBuilder GetByEmail(string userAreaCode, string emailAddress)
    {
        return new ContentRepositoryUserByEmailQueryBuilder(ExtendableContentRepository, userAreaCode, emailAddress);
    }

    public IContentRepositoryUserByEmailQueryBuilder GetByEmail<TUserArea>(string emailAddress) where TUserArea : IUserAreaDefinition
    {
        var userArea = GetUserAreaByType<TUserArea>();
        return new ContentRepositoryUserByEmailQueryBuilder(ExtendableContentRepository, userArea.UserAreaCode, emailAddress);
    }

    public IContentRepositoryUserByUsernameQueryBuilder GetByUsername(string userAreaCode, string username)
    {
        return new ContentRepositoryUserByUsernameQueryBuilder(ExtendableContentRepository, userAreaCode, username);
    }

    public IContentRepositoryUserByUsernameQueryBuilder GetByUsername<TUserArea>(string username) where TUserArea : IUserAreaDefinition
    {
        var userArea = GetUserAreaByType<TUserArea>();
        return new ContentRepositoryUserByUsernameQueryBuilder(ExtendableContentRepository, userArea.UserAreaCode, username);
    }

    public IContentRepositoryUserSearchQueryBuilder Search()
    {
        return new ContentRepositoryUserSearchQueryBuilder(ExtendableContentRepository);
    }

    public IContentRepositoryCurrentUserRepository Current()
    {
        return new ContentRepositoryCurrentUserRepository(ExtendableContentRepository);
    }

    private IUserAreaDefinition GetUserAreaByType<TUserArea>() where TUserArea : IUserAreaDefinition
    {
        var userAreaDefinitionRepository = ExtendableContentRepository.ServiceProvider.GetRequiredService<IUserAreaDefinitionRepository>();
        return userAreaDefinitionRepository.GetRequired<TUserArea>();
    }
}
