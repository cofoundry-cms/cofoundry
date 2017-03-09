using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Query to return a complete tree of web directory nodes, starting
    /// with the root webdirectory as a single node.
    /// </summary>
    public class GetWebDirectoryTreeQuery : IQuery<WebDirectoryNode>
    {
    }
}
