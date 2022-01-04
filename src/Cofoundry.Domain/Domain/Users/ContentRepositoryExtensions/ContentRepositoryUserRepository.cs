using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal
{
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

        public IContentRepositoryUserByUsernameQueryBuilder GetByUsername(string userAreaCode, string username)
        {
            return new ContentRepositoryUserByUsernameQueryBuilder(ExtendableContentRepository, userAreaCode, username);
        }

        public IContentRepositoryUserSearchQueryBuilder Search()
        {
            return new ContentRepositoryUserSearchQueryBuilder(ExtendableContentRepository);
        }

        public IContentRepositoryCurrentUserRepository Current()
        {
            return new ContentRepositoryCurrentUserRepository(ExtendableContentRepository);
        }
    }
}
