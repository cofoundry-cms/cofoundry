using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.BasicTestSite
{
    public class CarouselItemDataModel : INestedDataModel
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        [MultiLineText]
        [MaxLength(200)]
        public string Summary { get; set; }

        [Required]
        [MaxLength(200)]
        public string Url { get; set; }

        [Image]
        public int ImageId { get; set; }
    }
}
