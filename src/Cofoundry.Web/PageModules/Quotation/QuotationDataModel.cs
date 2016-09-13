using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Cofoundry.Domain;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Web
{
    public class QuotationDataModel : IPageModuleDataModel
    {
        [Display(Name = "Title (optional)")]
        [StringLength(128)]
        public string Title { get; set; }

        //TODO: [Searchable]
        [Required]
        [Display(Name = "Quotation text")]
        public string Quotation { get; set; }

        [Display(Name = "Citation text (optional)")]
        [StringLength(128)]
        public string CitationText { get; set; }

        [Display(Name = "Citation url")]
        [StringLength(256)]
        public string CitationUrl { get; set; }
    }
}