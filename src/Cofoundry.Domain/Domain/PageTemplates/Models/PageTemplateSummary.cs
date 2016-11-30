using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageTemplateSummary
    {
        public int PageTemplateId { get; set; }

        public string FileName { get; set; }

        public string Name { get; set; }

        public PageType PageType { get; set; }

        public int NumSections { get; set; }

        public int NumPages { get; set; }

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
