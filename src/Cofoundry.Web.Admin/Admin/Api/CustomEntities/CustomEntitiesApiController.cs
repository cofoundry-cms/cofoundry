using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.OData;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Cofoundry.Web.WebApi;
using Cofoundry.Core;

namespace Cofoundry.Web.Admin
{
    [AdminApiRoutePrefix("custom-entities")]
    public class CustomEntitiesApiController : BaseAdminApiController
    {
        #region private member variables

        private const string ID_ROUTE = "{customEntityId:int}";

        private readonly IQueryExecutor _queryExecutor;
        private readonly ApiResponseHelper _apiResponseHelper;

        #endregion

        #region constructor

        public CustomEntitiesApiController(
            IQueryExecutor queryExecutor,
            ApiResponseHelper apiResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _apiResponseHelper = apiResponseHelper;
        }

        #endregion

        #region routes

        #region queries

        [HttpGet]
        [Route]
        public async Task<IHttpActionResult> Get([FromUri] SearchCustomEntitySummariesQuery query, [FromUri] GetByIdRangeQuery<CustomEntitySummary> rangeQuery)
        {
            if (rangeQuery != null && rangeQuery.Ids != null)
            {
                var rangeResults = await _queryExecutor.ExecuteAsync(rangeQuery);
                return _apiResponseHelper.SimpleQueryResponse(this, rangeResults.ToFilteredAndOrderedCollection(rangeQuery.Ids));
            }

            if (query == null) query = new SearchCustomEntitySummariesQuery();

            var results = await _queryExecutor.ExecuteAsync(query);
            return _apiResponseHelper.SimpleQueryResponse(this, results);
        }

        [HttpGet]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Get(int customEntityId)
        {
            var result = await _queryExecutor.GetByIdAsync<CustomEntityDetails>(customEntityId);
            return _apiResponseHelper.SimpleQueryResponse(this, result);
        }

        #endregion

        #region commands

        [HttpPost]
        [Route()]
        public async Task<IHttpActionResult> Post([ModelBinder(typeof(CustomEntityDataModelCommandModelBinder))] AddCustomEntityCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut]
        [Route("ordering")]
        public async Task<IHttpActionResult> PutOrdering(ReOrderCustomEntitiesCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpPut]
        [Route(ID_ROUTE + "/url")]
        public async Task<IHttpActionResult> PutCustomEntityUrl(int customEntityId, UpdateCustomEntityUrlCommand command)
        {
            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        [HttpDelete]
        [Route(ID_ROUTE)]
        public async Task<IHttpActionResult> Delete(int customEntityId)
        {
            var command = new DeleteCustomEntityCommand();
            command.CustomEntityId = customEntityId;

            return await _apiResponseHelper.RunCommandAsync(this, command);
        }

        #endregion

        #endregion
    }
}