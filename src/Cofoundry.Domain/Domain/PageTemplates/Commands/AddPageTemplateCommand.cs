using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain
{
    public class AddPageTemplateCommand : ICommand, ILoggableCommand
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullPath { get; set; }

        public string Description { get; set; }

        public PageType PageType { get; set; }

        [MaxLength(100)]
        public string CustomEntityModelType { get; set; }

        [ValidateObject]
        public AddPageTemplateSectionWithPageTemplateCommand[] Sections { get; set; }

        [OutputValue]
        public int OutputPageTemplateId { get; set; }
    }
}
