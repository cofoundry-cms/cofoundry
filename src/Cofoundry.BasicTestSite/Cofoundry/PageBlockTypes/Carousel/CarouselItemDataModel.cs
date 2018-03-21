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
        [PreviewTitle]
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [PreviewDescription]
        [Required]
        [MultiLineText]
        [MaxLength(200)]
        public string Summary { get; set; }

        [Required]
        [MaxLength(200)]
        public string Url { get; set; }

        [PreviewImage]
        [Image]
        public int ImageId { get; set; }
    }
}
