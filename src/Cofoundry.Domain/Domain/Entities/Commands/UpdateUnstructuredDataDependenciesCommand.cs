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
    public class UpdateUnstructuredDataDependenciesCommand : ICommand
    {
        public UpdateUnstructuredDataDependenciesCommand(string rootEntityDefinitionCode, int rootEntityId, object model)
        {
            RootEntityDefinitionCode = rootEntityDefinitionCode;
            RootEntityId = rootEntityId;
            Model = model;
        }

        [Required]
        public string RootEntityDefinitionCode { get; set; }

        [Required]
        [PositiveInteger]
        public int RootEntityId { get; set; }

        [Required]
        public object Model { get; set; }
    }
}
