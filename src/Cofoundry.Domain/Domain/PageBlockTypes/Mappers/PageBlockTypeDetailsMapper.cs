using Cofoundry.Core;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to PageBlockTypeDetails objects.
    /// </summary>
    public class PageBlockTypeDetailsMapper : IPageBlockTypeDetailsMapper
    {
        private readonly IDynamicDataModelSchemaMapper _dynamicDataModelTypeMapper;
        private readonly IEnumerable<IPageBlockTypeDataModel> _allPageBlockTypeDataModels;

        public PageBlockTypeDetailsMapper(
            IDynamicDataModelSchemaMapper dynamicDataModelTypeMapper,
            IEnumerable<IPageBlockTypeDataModel> allPageBlockTypeDataModels
            )
        {
            _dynamicDataModelTypeMapper = dynamicDataModelTypeMapper;
            _allPageBlockTypeDataModels = allPageBlockTypeDataModels;
        }

        /// <summary>
        /// Maps an EF PageBlockType record from the db into an PageBlockTypeDetails 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="blockTypeSummary">PageBlockType record from the database.</param>
        public PageBlockTypeDetails Map(PageBlockTypeSummary blockTypeSummary)
        {
            var result = new PageBlockTypeDetails()
            {
                Description = blockTypeSummary.Description,
                FileName = blockTypeSummary.FileName,
                Name = blockTypeSummary.Name,
                PageBlockTypeId = blockTypeSummary.PageBlockTypeId,
                Templates = blockTypeSummary.Templates
            };

            var dataModelType = GetPageBlockDataModelType(result);

            _dynamicDataModelTypeMapper.Map(result, dataModelType);

            return result;
        }

        private Type GetPageBlockDataModelType(PageBlockTypeDetails blockTypeDetails)
        {
            var dataModelName = blockTypeDetails.FileName + "DataModel";

            var dataModel = _allPageBlockTypeDataModels
                .Select(m => m.GetType())
                .Where(m => m.Name == dataModelName)
                .SingleOrDefault();

            EntityNotFoundException.ThrowIfNull(dataModel, dataModelName);

            return dataModel;
        }
    }
}
