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

        [Display(Description = "By default the document will display 'inline' in the browser, but you can change this behavior to instead force the browser to download the document.")]
        [RadioList(typeof(DocumentDownloadMode))]
        public DocumentDownloadMode DownloadMode { get; set; }
    }
}