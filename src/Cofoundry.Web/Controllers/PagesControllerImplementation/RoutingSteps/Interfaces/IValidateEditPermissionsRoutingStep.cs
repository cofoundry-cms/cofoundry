using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web
{
    /// <summary>
    /// Checks to see if the user has permissions to update a page/entity
    /// and downgrades the edititor mode to draft view if the permissions are 
    /// missing.
    /// </summary>
    public interface IValidateEditPermissionsRoutingStep : IPageActionRoutingStep
    {
    }
}
