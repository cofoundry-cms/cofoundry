using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateDetails
    {
        public int PageTemplateId { get; set; }

        public string FileName { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string FullPath { get; set; }

        public int NumPages { get; set; }

        public CustomEntityDefinitionMicroSummary CustomEntityDefinition { get; set; }

        public string CustomEntityModelType { get; set; }

        public PageType PageType { get; set; }

        public PageTemplateSectionDetails[] Sections { get; set; }

        public bool IsArchived { get; set; }

        #region Auditing

        /// <summary>
        /// The date the template was created
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// The date the template was last updated
        /// </summary>
        public DateTime UpdateDate { get; set; }

        #endregion
    }
}
