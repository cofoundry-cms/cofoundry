﻿namespace Cofoundry.Web;

/// <summary>
/// Simple implementation of IPageWithMetaDataViewModel for pages that don't need much else.
/// </summary>
public class SimplePageViewModel : IPageWithMetaDataViewModel
{
    public string PageTitle { get; set; } = string.Empty;

    public string? MetaDescription { get; set; }
}
