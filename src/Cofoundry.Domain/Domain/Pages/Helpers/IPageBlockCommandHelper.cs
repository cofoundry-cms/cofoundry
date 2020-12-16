using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Cofoundry.Domain.Data;
using Cofoundry.Core;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Shared helper between add/update page block commands for updating the db model
    /// </summary>
    public interface IPageBlockCommandHelper
    {
        Task UpdateModelAsync(IPageVersionBlockDataModelCommand command, IEntityVersionPageBlock dbModule);
    }
}
