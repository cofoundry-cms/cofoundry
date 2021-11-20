using Cofoundry.Core.Validation;
using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    /// <summary>
    /// Here we can use ILoggableCommand to mark this class as
    /// loggable. The default logger only logs to the debugger,
    /// but you can use a plugin to extend this functionality
    /// </summary>
    public class SetCatLikedCommand : ICommand, ILoggableCommand
    {
        [PositiveInteger]
        [Required]
        public int CatId { get; set; }

        public bool IsLiked { get; set; }
    }

}
