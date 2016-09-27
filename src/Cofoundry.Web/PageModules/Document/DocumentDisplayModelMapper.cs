using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Core;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;

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

        public IEnumerable<PageModuleDisplayModelMapperOutput> Map(IEnumerable<PageModuleDisplayModelMapperInput<DocumentDataModel>> inputs, WorkFlowStatusQuery workflowStatus)
        {
            var documents = _queryExecutor.GetByIdRange<DocumentAssetRenderDetails>(inputs.Select(i => i.DataModel.DocumentAssetId));

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

                yield return input.CreateOutput(output);
            }
        }
    }
}