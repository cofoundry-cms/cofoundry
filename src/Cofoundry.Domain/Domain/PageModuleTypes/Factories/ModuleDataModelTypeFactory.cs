using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    public class ModuleDataModelTypeFactory : IModuleDataModelTypeFactory
    {
        #region constructor

        private readonly IPageModuleDataModel[] _allPageModuleDataModels;
        private readonly IQueryExecutor _queryExecutor;
        private readonly IPageModuleTypeFileNameFormatter _moduleTypeFileNameFormatter;

        public ModuleDataModelTypeFactory(
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

        public Type CreateByPageModuleTypeName(string moduleTypeName)
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

        public Type CreateByPageModuleTypeId(int pageModuleTypeId)
        {
            var moduleType = _queryExecutor.GetById<PageModuleTypeSummary>(pageModuleTypeId);
            EntityNotFoundException.ThrowIfNull(moduleType, pageModuleTypeId);

            return CreateByPageModuleTypeName(moduleType.FileName);
        }
    }
}
