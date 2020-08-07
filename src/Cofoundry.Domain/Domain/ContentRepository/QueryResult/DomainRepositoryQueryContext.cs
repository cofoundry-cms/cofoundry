using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class DomainRepositoryQueryContext<TResult> : IDomainRepositoryQueryContext<TResult>
    {
        public DomainRepositoryQueryContext(
            IQuery<TResult> query,
            IExtendableContentRepository extendableRepository
            )
        {
            Query = query;
            ExtendableContentRepository = extendableRepository;
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
