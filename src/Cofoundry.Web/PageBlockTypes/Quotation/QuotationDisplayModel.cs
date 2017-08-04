using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class QuotationDisplayModel : IPageBlockTypeDisplayModel
    {
        public string Title { get; set; }
        public string Quotation { get; set; }
        public string CitationText { get; set; }
        public string CitationUrl { get; set; }
    }
}