using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain;

namespace Cofoundry.Domain
{
    public static class VisualEditorModeExtensions
    {
        /// <summary>
        /// Converts the user requested VisualEditorMode into the equivalent
        /// PublishStatusQuery which can be used for domain querying.
        /// </summary>
        public static PublishStatusQuery ToPublishStatusQuery(this VisualEditorMode visualEditorMode)
        {
            switch (visualEditorMode)
            {
                case VisualEditorMode.Preview:
                case VisualEditorMode.Edit:
                    return PublishStatusQuery.Draft;
                case VisualEditorMode.Any:
                    return PublishStatusQuery.Latest;
                case VisualEditorMode.SpecificVersion:
                    return PublishStatusQuery.SpecificVersion;
                default:
                    return PublishStatusQuery.Published;
            }
        }

        /// <summary>
        /// Gets the publish status that should be used to query any
        /// entities that are not the primary target of the visual 
        /// editor, e.g. a related entity or entities not directly associated 
        /// with an entity being edited. 
        /// </summary>
        /// <remarks>
        /// In most cases this would be the same for both primary and ambient 
        /// entity, but in some cases it is different e.g. if the primary 
        /// entity has been requested as a specific version then ambient entities
        /// cannot also be a specific version and should show the latest.
        /// </remarks>
        public static PublishStatusQuery ToAmbientEntityPublishStatusQuery(this VisualEditorMode visualEditorMode)
        {
            if (visualEditorMode == VisualEditorMode.Live)
            {
                return PublishStatusQuery.Published;
            }

            return PublishStatusQuery.Latest;
        }
    }
}
