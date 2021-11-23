using Cofoundry.Domain.CQS;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    public class ImageAssetEntityDefinition : IDependableEntityDefinition
    {
        public const string DefinitionCode = "COFIMG";

        public string EntityDefinitionCode => DefinitionCode;

        public string Name => "Image";

        public IQuery<IDictionary<int, RootEntityMicroSummary>> CreateGetEntityMicroSummariesByIdRangeQuery(IEnumerable<int> ids)
        {
            return new GetImageAssetEntityMicroSummariesByIdRangeQuery(ids);
        }
    }
}
