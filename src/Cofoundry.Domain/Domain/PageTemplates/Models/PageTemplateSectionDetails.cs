using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateSectionDetails
    {
        public int PageTemplateSectionId { get; set; }

        public int PageTemplateId { get; set; }

        public string Name { get; set; }

        public bool IsCustomEntitySection { get; set; }

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
