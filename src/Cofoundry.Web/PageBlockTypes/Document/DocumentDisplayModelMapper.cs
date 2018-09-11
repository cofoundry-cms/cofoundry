using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Core;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class DocumentDisplayModelMapper : IPageBlockTypeDisplayModelMapper<DocumentDataModel>
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

        public async Task MapAsync(
            PageBlockTypeDisplayModelMapperContext<DocumentDataModel> context, 
            PageBlockTypeDisplayModelMapperResult<DocumentDataModel> result
            )
        {
            var documentIds = context.Items.SelectDistinctModelValuesWithoutEmpty(i => i.DocumentAssetId);
            var documentsQuery = new GetDocumentAssetRenderDetailsByIdRangeQuery(documentIds);
            var documents = await _queryExecutor.ExecuteAsync(documentsQuery, context.ExecutionContext);

            foreach (var item in context.Items)
            {
                var document = documents.GetOrDefault(item.DataModel.DocumentAssetId);

                var displayModel = new DocumentDisplayModel();
                if (document != null)
                {
                    displayModel.Description = document.Description;
                    displayModel.Title = document.Title;
                    if (item.DataModel.DownloadMode == DocumentDownloadMode.ForceDownload)
                    {
                        displayModel.Url = _documentAssetRouteLibrary.DocumentAssetDownload(document);
                    }
                    else
                    {
                        displayModel.Url = _documentAssetRouteLibrary.DocumentAsset(document);
                    }
                }

                result.Add(item, displayModel);
            }
        }
    }
}