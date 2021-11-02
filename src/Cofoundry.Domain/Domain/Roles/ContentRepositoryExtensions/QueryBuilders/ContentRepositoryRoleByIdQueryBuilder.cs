using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryRoleByIdQueryBuilder
        : IContentRepositoryRoleByIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int? _roleId;

        public ContentRepositoryRoleByIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int? roleId
            )
        {
            ExtendableContentRepository = contentRepository;
            _roleId = roleId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<RoleMicroSummary> AsMicroSummary()
        {
            var query = new GetRoleMicroSummaryByIdQuery(_roleId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<RoleDetails> AsDetails()
        {
            var query = new GetRoleDetailsByIdQuery(_roleId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
