using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Web
{
    /// <summary>
    /// A state object used to store data relating to the visual editor.
    /// Once created, this is typically this is cached for the duration 
    /// of the request.
    /// </summary>
    public class VisualEditorState
    {
        public VisualEditorState()
            : this(VisualEditorMode.Live)
        {
        }

        public VisualEditorState(
            VisualEditorMode visualEditorMode
            )
        {
            VisualEditorMode = visualEditorMode;
        }

        /// <summary>
        /// The visual editor mode applying to page or entity that is
        /// being edited.
        /// </summary>
        public VisualEditorMode VisualEditorMode { get; private set; }

        /// <summary>
        /// Converts the user requested VisualEditorMode into the equivalent
        /// PublishStatusQuery which can be used for domain querying of the target
        /// entity (e.g. page). For entities not directly related to the target entity
        /// you can instead use GetAmbientEntityPublishStatusQuery()
        /// </summary>
        public PublishStatusQuery GetPublishStatusQuery()
        {
            return VisualEditorMode.ToPublishStatusQuery();
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
        public PublishStatusQuery GetAmbientEntityPublishStatusQuery()
        {
            return VisualEditorMode.ToAmbientEntityPublishStatusQuery();
        }
    }
}
