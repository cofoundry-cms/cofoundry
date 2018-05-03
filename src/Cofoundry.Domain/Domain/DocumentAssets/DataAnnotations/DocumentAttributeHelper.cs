using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Helper for shared code used in the document data annotions.
    /// </summary>
    public static class DocumentAttributeMetaDataHelper
    {
        public static void AddFilterData(
            DisplayMetadata modelMetaData, 
            ICollection<string> fileExtensions,
            ICollection<string> tags
            )
        {
            modelMetaData.AddAdditionalValueIfNotEmpty("Tags", tags);
            if (fileExtensions != null && fileExtensions.Count == 1)
            {
                modelMetaData.AddAdditionalValueIfNotEmpty("FileExtension", fileExtensions.First());
            }
            else if (fileExtensions != null)
            {
                modelMetaData.AddAdditionalValueIfNotEmpty("FileExtensions", string.Join(", ", fileExtensions));
            }
        }
    }
}
