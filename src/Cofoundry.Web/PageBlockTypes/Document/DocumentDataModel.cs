using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    /// <summary>
    /// Data model representing a link to download a document asset
    /// </summary>
    public class DocumentDataModel : IPageBlockTypeDataModel
    {
        [Display(Name = "Document")]
        [Required]
        [Document]
        public int DocumentAssetId { get; set; }
    }
}