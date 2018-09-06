using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Publishes a page. If the page is already published and
    /// a date is specified then the publish date will be updated.
    /// </summary>
    public class PublishPageCommand : ICommand, ILoggableCommand
    {
        /// <summary>
        /// Publishes a page. If the page is already published and
        /// a date is specified then the publish date will be updated.
        /// </summary>
        public PublishPageCommand()
        {
        }

        /// <summary>
        /// Publishes a page. If the page is already published and
        /// a date is specified then the publish date will be updated.
        /// </summary>
        /// <param name="pageId">The database id of the page to publish.</param>
        /// <param name="publishDate">
        /// Optional date-time that the page should be published and made public. If
        /// this is left null then the publish date is set to the current 
        /// date and the page is made immediately available.
        /// </param>
        public PublishPageCommand(int pageId, DateTime? publishDate = null)
        {
            PageId = pageId;
            PublishDate = publishDate;
        }
        
        /// <summary>
        /// The database id of the page to publish.
        /// </summary>
        [PositiveInteger]
        [Required]
        public int PageId { get; set; }

        /// <summary>
        /// Optional time that the page should be published and made public. If
        /// this is left null then the publish date is set to the current 
        /// date and the page is made immediately available.
        /// </summary>
        public DateTime? PublishDate { get; set; }
    }
}
