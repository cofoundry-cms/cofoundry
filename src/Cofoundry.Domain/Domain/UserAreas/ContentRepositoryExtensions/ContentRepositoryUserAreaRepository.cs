using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryUserAreaRepository
            : IAdvancedContentRepositoryUserAreaRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryUserAreaRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryUserAreaGetAllQueryBuilder GetAll()
        {
            return new ContentRepositoryUserAreaGetAllQueryBuilder(ExtendableContentRepository);
        }

        public IContentRepositoryUserAreaByCodeQueryBuilder GetByCode(string userAreaCode)
        {
            return new ContentRepositoryUserAreaByCodeQueryBuilder(ExtendableContentRepository, userAreaCode);
        }
    }
}
