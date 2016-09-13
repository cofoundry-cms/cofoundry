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
    public class PublishCustomEntityCommand : ICommand, ILoggableCommand
    {
        public PublishCustomEntityCommand()
        {
        }

        public PublishCustomEntityCommand(int customEntityId)
        {
            CustomEntityId = customEntityId;
        }

        [PositiveInteger]
        [Required]
        public int CustomEntityId { get; set; }
    }
}
