using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryCustomEntityDefinitionGetAllQueryBuilder
        : IContentRepositoryCustomEntityDefinitionGetAllQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCustomEntityDefinitionGetAllQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<ICollection<CustomEntityDefinitionMicroSummary>> AsMicroSummaryAsync()
        {
            var query = new GetAllCustomEntityDefinitionMicroSummariesQuery();
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        public Task<ICollection<CustomEntityDefinitionSummary>> AsSummaryAsync()
        {
            var query = new GetAllCustomEntityDefinitionSummariesQuery();
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
