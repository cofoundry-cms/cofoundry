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
    /// Extracts information about a page block type from a view file with
    /// the specified name. If the file is not found then null is returned.
    /// </summary>
    public class GetPageBlockTypeFileDetailsByFileNameQuery : IQuery<PageBlockTypeFileDetails>
    {
        public GetPageBlockTypeFileDetailsByFileNameQuery() { }

        public GetPageBlockTypeFileDetailsByFileNameQuery(string fileName)
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
