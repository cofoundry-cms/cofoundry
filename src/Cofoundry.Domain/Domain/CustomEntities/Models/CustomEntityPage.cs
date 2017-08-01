using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityPage
    {
        public PageRoute PageRoute { get; set; }

        /// <summary>
        /// The full path of the page including directories and the locale. 
        /// </summary>
        public string FullPath { get; set; }

        public IEnumerable<CustomEntityPageSectionDetails> Sections { get; set; }
    }
}
