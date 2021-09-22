using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageDirectoryNodeByIdQuery : IQuery<PageDirectoryNode>
    {
        public GetPageDirectoryNodeByIdQuery()
        {
        }

        public GetPageDirectoryNodeByIdQuery(int pageDirectoryId)
        {
            PageDirectoryId = pageDirectoryId;
        }

        public int PageDirectoryId { get; set; }
    }
}
