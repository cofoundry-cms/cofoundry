using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to PageBlockTypeSummary objects.
    /// </summary>
    public class PageBlockTypeSummaryMapper : IPageBlockTypeSummaryMapper
    {
        /// <summary>
        /// Maps an EF PageBlockType record from the db into an PageBlockTypeSummary 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbPageBlockType">PageBlockType record from the database.</param>
        public PageBlockTypeSummary Map(PageBlockType dbPageBlockType)
        {
            var result = new PageBlockTypeSummary()
            {
                Description = dbPageBlockType.Description,
                FileName = dbPageBlockType.FileName,
                Name = dbPageBlockType.Name,
                PageBlockTypeId = dbPageBlockType.PageBlockTypeId
            };

            result.Templates = dbPageBlockType
                .PageBlockTemplates
                .Select(Map)
                .ToList();

            return result;
        }

        private PageBlockTypeTemplateSummary Map(PageBlockTypeTemplate dbTemplate)
        {
            var result = new PageBlockTypeTemplateSummary()
            {
                FileName = dbTemplate.FileName,
                Name = dbTemplate.Name,
                PageBlockTypeTemplateId = dbTemplate.PageBlockTypeTemplateId
            };

            return result;
        }
    }
}
