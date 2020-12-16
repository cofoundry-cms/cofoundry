using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Used to create data model types and validate they exist
    /// </summary>
    public class PageBlockTypeDataModelTypeFactory : IPageBlockTypeDataModelTypeFactory
    {
        #region constructor

        private readonly IEnumerable<IPageBlockTypeDataModel> _allPageBlockTypeDataModels;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageBlockTypeFileNameFormatter _pageBlockTypeFileNameFormatter;

        public PageBlockTypeDataModelTypeFactory(
            IEnumerable<IPageBlockTypeDataModel> allPageBlockTypeDataModels,
            IQueryExecutor queryExecutor,
            IPageBlockTypeFileNameFormatter pageBlockTypeFileNameFormatter
            )
        {
            _allPageBlockTypeDataModels = allPageBlockTypeDataModels;
            _queryExecutor = queryExecutor;
            _pageBlockTypeFileNameFormatter = pageBlockTypeFileNameFormatter;
        }

        #endregion

        /// <summary>
        /// Creates a data model type from the file name
        /// string i.e. 'PlainText' not 'PlainTextDataModel'. Throws 
        /// an InvalidOperationException if the requested type is not register
        /// or has been defined multiple times.
        /// </summary>
        /// <param name="pageBlockTypeFileName">The unique name of the page block type.</param>
        public Type CreateByPageBlockTypeFileName(string pageBlockTypeFileName)
        {
            // take advantage of the cached type list in the dependency resolver to get a collection of DataModel instances
            // we dont actually use these instances, we just use them to get the type. A bit wasteful perhaps but object creation is cheap 
            // and the only alternative is searching through all assembly types which is very slow.
            var dataModelTypes = _allPageBlockTypeDataModels
                .Select(t => t.GetType())
                .Where(t => _pageBlockTypeFileNameFormatter.FormatFromDataModelType(t).Equals(pageBlockTypeFileName, StringComparison.OrdinalIgnoreCase));

            if (!dataModelTypes.Any())
            {
                throw new InvalidOperationException(string.Format("DataModel for page block type {0} not yet implemented", pageBlockTypeFileName));
            }

            if (dataModelTypes.Count() > 1)
            {
                throw new InvalidOperationException(string.Format("DataModel for page block type {0} is registered multiple times. Please use unique class names.", pageBlockTypeFileName));
            }

            return dataModelTypes.First();
        }

        /// <summary>
        /// Creates a data model type from the database id. Throws 
        /// an InvalidOperationException if the requested type is not register
        /// or has been defined multiple times
        /// </summary>
        /// <param name="pageBlockTypeId">Id of the page block type in the database</param>
        public async Task<Type> CreateByPageBlockTypeIdAsync(int pageBlockTypeId)
        {
            var query = new GetPageBlockTypeSummaryByIdQuery(pageBlockTypeId);
            var blockType = await _queryExecutor.ExecuteAsync(query);
            EntityNotFoundException.ThrowIfNull(blockType, pageBlockTypeId);

            return CreateByPageBlockTypeFileName(blockType.FileName);
        }
    }
}
