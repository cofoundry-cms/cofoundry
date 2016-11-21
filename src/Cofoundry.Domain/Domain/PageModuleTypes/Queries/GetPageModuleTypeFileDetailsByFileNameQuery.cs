using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Extracts information about a page module from a view file with
    /// the specified name. If the file is not found then null is returned.
    /// </summary>
    public class GetPageModuleTypeFileDetailsByFileNameQuery : IQuery<PageModuleTypeFileDetails>
    {
        public GetPageModuleTypeFileDetailsByFileNameQuery() { }

        public GetPageModuleTypeFileDetailsByFileNameQuery(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        /// Full path including filename and file extension of the page
        /// template view file to parse.
        /// </summary>
        public string FileName { get; set; }
    }
}
