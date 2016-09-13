using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetPageTemplateFileInfoByPathQuery : IQuery<PageTemplateFileInfo>
    {
        public string FullPath { get; set; }
    }
}
