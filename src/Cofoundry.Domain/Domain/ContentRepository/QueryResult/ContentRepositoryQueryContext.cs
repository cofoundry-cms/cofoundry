using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryQueryContext<TResult> : IContentRepositoryQueryContext<TResult>
    {
        public ContentRepositoryQueryContext(
            IQuery<TResult> query,
            IExtendableContentRepository contentRepository
            )
        {
            Query = query;
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IQuery<TResult> Query { get; }

        public async Task<TResult> ExecuteAsync()
        {
            var result = await ExtendableContentRepository.ExecuteQueryAsync(Query);

            return result;
        }
    }
}
