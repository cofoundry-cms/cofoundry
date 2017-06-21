using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class DocumentDisplayModelMapper : IPageModuleDisplayModelMapper<DocumentDataModel>
    {
        #region Constructor

        private IQueryExecutor _queryExecutor;
        private IDocumentAssetRouteLibrary _documentAssetRouteLibrary;

        public DocumentDisplayModelMapper(
            IQueryExecutor queryExecutor,
            IDocumentAssetRouteLibrary documentAssetRouteLibrary
            )
        {
            _queryExecutor = queryExecutor;
            _documentAssetRouteLibrary = documentAssetRouteLibrary;
        }

        #endregion

        public async Task<IEnumerable<PageModuleDisplayModelMapperOutput>> MapAsync(IEnumerable<PageModuleDisplayModelMapperInput<DocumentDataModel>> inputs, WorkFlowStatusQuery workflowStatus)
        {
            var documents = await _queryExecutor.GetByIdRangeAsync<DocumentAssetRenderDetails>(inputs.Select(i => i.DataModel.DocumentAssetId));
            var results = new List<PageModuleDisplayModelMapperOutput>();

            foreach (var input in inputs)
            {
                var document = documents.GetOrDefault(input.DataModel.DocumentAssetId);

                var output = new DocumentDisplayModel();
                if (document != null)
                {
                    output.Description = document.Description;
                    output.Title = document.Title;
                    output.Url = _documentAssetRouteLibrary.DocumentAsset(document);
                }

                results.Add(input.CreateOutput(output));
            }

            return results.AsEnumerable();
        }
    }
}