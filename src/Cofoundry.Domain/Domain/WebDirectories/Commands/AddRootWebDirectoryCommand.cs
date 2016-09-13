using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class AddRootWebDirectoryCommand : ICommand, ILoggableCommand
    {
        #region Output

        [OutputValue]
        public int OutputWebDirectoryId { get; set; }

        #endregion
    }
}
