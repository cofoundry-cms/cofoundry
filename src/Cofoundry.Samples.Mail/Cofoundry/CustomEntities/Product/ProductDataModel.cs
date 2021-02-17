using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.Samples.Mail
{
    public class ProductDataModel : ICustomEntityDataModel
    {
        [MaxLength(500)]
        [Display(Description = "A short description of the product.")]
        [MultiLineText]
        public string ShortDescription { get; set; }
    }
}