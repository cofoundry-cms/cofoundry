namespace Cofoundry.Domain;

public static class VisualEditorModeExtensions
{
    /// <summary>
    /// Converts the user requested VisualEditorMode into the equivalent
    /// PublishStatusQuery which can be used for domain querying.
    /// </summary>
    public static PublishStatusQuery ToPublishStatusQuery(this VisualEditorMode visualEditorMode)
    {
        return visualEditorMode switch
        {
            VisualEditorMode.Preview or VisualEditorMode.Edit => PublishStatusQuery.Draft,
            VisualEditorMode.Any => PublishStatusQuery.Latest,
            VisualEditorMode.SpecificVersion => PublishStatusQuery.SpecificVersion,
            _ => PublishStatusQuery.Published,
        };
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
