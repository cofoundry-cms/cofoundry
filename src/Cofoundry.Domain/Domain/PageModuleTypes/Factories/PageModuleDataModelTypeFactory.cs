using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to create data model types and validate they exist
    /// </summary>
    public class PageModuleDataModelTypeFactory : IPageModuleDataModelTypeFactory
    {
        #region constructor

        private readonly IPageModuleDataModel[] _allPageModuleDataModels;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageModuleTypeFileNameFormatter _moduleTypeFileNameFormatter;

        public PageModuleDataModelTypeFactory(
            IPageModuleDataModel[] allPageModuleDataModels,
            IQueryExecutor queryExecutor,
            IPageModuleTypeFileNameFormatter moduleTypeFileNameFormatter
            )
        {
            _allPageModuleDataModels = allPageModuleDataModels;
            _queryExecutor = queryExecutor;
            _moduleTypeFileNameFormatter = moduleTypeFileNameFormatter;
        }

        #endregion

        /// <summary>
        /// Creates a data model type from the file name
        /// string i.e. 'PlainText' not 'PlainTextDataModel'. Throws 
        /// an InvalidOperationException if the requested type is not register
        /// or has been defined multiple times
        /// </summary>
        /// <param name="moduleTypeFileName">The unique name of the module type</param>
        public Type CreateByPageModuleTypeFileName(string moduleTypeName)
        {
            // take advantage of the cached type list in the dependency resolver to get a collection of DataModel instances
            // we dont actually use these instances, we just use them to get the type. A bit wasteful perhaps but object creation is cheap 
            // and the only alternative is searching through all assembly types which is very slow.
            var dataModelTypes = _allPageModuleDataModels
                .Select(t => t.GetType())
                .Where(t => _moduleTypeFileNameFormatter.FormatFromDataModelType(t).Equals(moduleTypeName, StringComparison.OrdinalIgnoreCase));

            if (!dataModelTypes.Any())
            {
                throw new InvalidOperationException(string.Format("DataModel for page module type {0} not yet implemented", moduleTypeName));
            }

            if (dataModelTypes.Count() > 1)
            {
                throw new InvalidOperationException(string.Format("DataModel for page module type {0} is registered multiple times. Please use unique class names.", moduleTypeName));
            }

            return dataModelTypes.First();
        }

        /// <summary>
        /// Creates a data model type from the database id. Throws 
        /// an InvalidOperationException if the requested type is not register
        /// or has been defined multiple times
        /// </summary>
        /// <param name="pageModuleTypeId">Id of the page module type in the database</param>
        public Type CreateByPageModuleTypeId(int pageModuleTypeId)
        {
            var moduleType = _queryExecutor.GetById<PageModuleTypeSummary>(pageModuleTypeId);
            EntityNotFoundException.ThrowIfNull(moduleType, pageModuleTypeId);

            return CreateByPageModuleTypeFileName(moduleType.FileName);
        }
    }
}
