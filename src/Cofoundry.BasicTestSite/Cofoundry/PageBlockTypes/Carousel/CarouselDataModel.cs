using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.BasicTestSite
{
    public class CarouselDataModel : IPageBlockTypeDataModel, IPageBlockTypeDisplayModel
    {
        [MaxLength(100)]
        [Required]
        public string Title { get; set; }

        [NestedDataModelCollection(IsOrderable = true, MinItems = 1, MaxItems = 3)]
        public ICollection<CarouselItemDataModel> Items { get; set; }
    }
}
