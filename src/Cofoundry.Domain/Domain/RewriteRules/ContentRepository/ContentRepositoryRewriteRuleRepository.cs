using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryRewriteRuleRepository
            : IAdvancedContentRepositoryRewriteRuleRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryRewriteRuleRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        #region queries

        public IContentRepositoryRewriteRuleByPathQueryBuilder GetByPath(string path)
        {
            return new ContentRepositoryRewriteRuleByPathQueryBuilder(ExtendableContentRepository, path);
        }

        public IContentRepositoryRewriteRuleGetAllQueryBuilder GetAll()
        {
            return new ContentRepositoryRewriteRuleGetAllQueryBuilder(ExtendableContentRepository);
        }

        #endregion

        #region commands

        public async Task<int> AddAsync(AddRedirectRuleCommand command)
        {
            await ExtendableContentRepository.ExecuteCommandAsync(command);
            return command.OutputRedirectRuleId;
        }

        #endregion
    }
}
