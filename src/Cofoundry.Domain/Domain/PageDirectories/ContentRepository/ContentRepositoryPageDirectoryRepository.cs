using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPageDirectoryRepository
            : IContentRepositoryPageDirectoryRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageDirectoryRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryPageDirectoryByIdQueryBuilder GetById(int pageDirectoryId)
        {
            return new ContentRepositoryPageDirectoryByIdQueryBuilder(ExtendableContentRepository, pageDirectoryId);
        }

        public IContentRepositoryPageDirectoryGetAllQueryBuilder GetAll()
        {
            return new ContentRepositoryPageDirectoryGetAllQueryBuilder(ExtendableContentRepository);
        }
    }
}
