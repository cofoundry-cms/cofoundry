﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Cofoundry.Domain;

/// <summary>
/// Helper for shared code used in the document data annotions.
/// </summary>
public static class DocumentAttributeMetaDataHelper
{
    public static void AddFilterData(
        DisplayMetadata modelMetaData,
        IReadOnlyCollection<string>? fileExtensions,
        IReadOnlyCollection<string>? tags
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
