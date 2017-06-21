using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain;

namespace Cofoundry.Web
{
    public class VisualEditorFrameModel
    {
        public bool IsInEditMode { get; set; }

        public int PageId { get; set; }

        public int VersionId { get; set; }

        public bool IsCustomEntityRoute { get; set; }

        public string EntityNameSingular { get; set; }

        public int EntityId { get; set; }

        public bool HasDraftVersion { get; set; }
    }
}