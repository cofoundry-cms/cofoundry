using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.Data;
using Cofoundry.Core;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Shared helper between add/update page module commands for updating the db model
    /// </summary>
    public interface IPageModuleCommandHelper
    {
        Task UpdateModelAsync(IPageVersionModuleDataModelCommand command, IEntityVersionPageModule dbModule);
    }
}
