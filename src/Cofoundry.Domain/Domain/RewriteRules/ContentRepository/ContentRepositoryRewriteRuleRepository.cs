using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
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

        public Task AddAsync(AddRedirectRuleCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        #endregion
    }
}
