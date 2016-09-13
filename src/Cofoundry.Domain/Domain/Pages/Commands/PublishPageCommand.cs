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
    public class PublishPageCommand : ICommand, ILoggableCommand
    {
        public PublishPageCommand()
        {
        }

        public PublishPageCommand(int pageId)
        {
            PageId = pageId;
        }
        
        [PositiveInteger]
        [Required]
        public int PageId { get; set; }
    }
}
