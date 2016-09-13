using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IPageRoute
    {
        /// <summary>
        /// The full path of the page including directories and the locale. 
        /// </summary>
        string FullPath { get; }
    }
}
