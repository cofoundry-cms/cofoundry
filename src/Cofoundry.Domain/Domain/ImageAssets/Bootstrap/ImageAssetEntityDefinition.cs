using Cofoundry.Domain.CQS;
using System.Collections.Generic;

namespace Cofoundry.Domain
{
    /// <summary>
    /// An image asset represents an image file that has been uploaded to the CMS.
    /// </summary>
    public sealed class ImageAssetEntityDefinition : IDependableEntityDefinition
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