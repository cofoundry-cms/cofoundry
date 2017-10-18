using Cofoundry.Core;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    public interface IPageDirectoryRouteMapper
    {
        List<PageDirectoryRoute> Map(IEnumerable<PageDirectory> dbPageDirectories);
    }
}
