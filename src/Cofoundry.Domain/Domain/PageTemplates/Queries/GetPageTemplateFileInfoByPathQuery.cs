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
    /// Extracts information about a page template from the template file at
    /// the specified path. If the file is not found then null is returned.
    /// </summary>
    public class GetPageTemplateFileInfoByPathQuery : IQuery<PageTemplateFileInfo>
    {
        public GetPageTemplateFileInfoByPathQuery() { }

        public GetPageTemplateFileInfoByPathQuery(string fullPath)
        {
            FullPath = fullPath;
        }

        /// <summary>
        /// Full path including filename and file extension of the page
        /// template view file to parse.
        /// </summary>
        public string FullPath { get; set; }
    }
}
