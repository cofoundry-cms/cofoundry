using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class AddRedirectRuleCommand : ICommand, ILoggableCommand
    {
        [StringLength(2000)]
        [Required]
        public string WriteFrom { get; set; }

        [StringLength(2000)]
        [Required]
        public string WriteTo { get; set; }

        #region Output

        [OutputValue]
        public int OutputRedirectRuleId { get; set; }

        #endregion
    }
}
