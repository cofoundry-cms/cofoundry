﻿namespace Cofoundry.Web.Admin;

public class VisualEditorFrameModel
{
    public bool IsInEditMode { get; set; }

    public int PageId { get; set; }

    public int VersionId { get; set; }

    public bool IsCustomEntityRoute { get; set; }

    public string EntityNameSingular { get; set; } = string.Empty;

    public int EntityId { get; set; }

    public bool HasDraftVersion { get; set; }
}
