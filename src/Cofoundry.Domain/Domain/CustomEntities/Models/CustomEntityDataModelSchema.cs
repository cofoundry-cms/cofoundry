﻿namespace Cofoundry.Domain;

/// <summary>
/// Meta information about a data model, including UI display details
/// and validation attributes for each public property. This is typically 
/// used for expressing these entities in dynamically generated parts of 
/// the UI e.g. edit forms and lists.
/// </summary>
public class CustomEntityDataModelSchema : IDynamicDataModelSchema
{
    /// <summary>
    /// The six character definition code that represents the type of custom
    /// entity e.g. Blog Post, Project, Product. The definition code is defined
    /// in a class that inherits from ICustomEntityDefinition.
    /// </summary>
    public string CustomEntityDefinitionCode { get; set; } = string.Empty;

    public string DataTemplateName { get; set; } = string.Empty;

    /// <summary>
    /// Data model property meta data, including UI display details
    /// and validation attributes. This is typically used for dynamically generating 
    /// parts of the admin UI.
    /// </summary>
    public IReadOnlyCollection<DynamicDataModelSchemaProperty> DataModelProperties { get; set; } = Array.Empty<DynamicDataModelSchemaProperty>();

    public DynamicDataModelDefaultValue DefaultValue { get; set; } = DynamicDataModelDefaultValue.Uninitialized;
}
