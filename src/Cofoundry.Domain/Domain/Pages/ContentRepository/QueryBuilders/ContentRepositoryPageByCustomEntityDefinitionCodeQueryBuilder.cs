using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryPageByCustomEntityDefinitionCodeQueryBuilder
        : IAdvancedContentRepositoryPageByCustomEntityDefinitionCodeQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly string _customEntityDefinitionCode;

        public ContentRepositoryPageByCustomEntityDefinitionCodeQueryBuilder(
            IExtendableContentRepository contentRepository,
            string customEntityDefinitionCode
            )
        {
            ExtendableContentRepository = contentRepository;
            _customEntityDefinitionCode = customEntityDefinitionCode;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<ICollection<PageRoute>> AsRoutesAsync()
        {
            var query = new GetPageRoutesByCustomEntityDefinitionCodeQuery(_customEntityDefinitionCode);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
