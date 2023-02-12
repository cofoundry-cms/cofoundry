namespace Cofoundry.Domain;

public class EntityExtensionDataModelSchema : IDynamicDataModelSchema
{
    /// <summary>
    /// prop name
    /// </summary>
    public string Name { get; set; }

    public string GroupName { get; set; }

    public string DataTemplateName { get; set; }

    public ICollection<DynamicDataModelSchemaProperty> DataModelProperties { get; set; }

    public DynamicDataModelDefaultValue DefaultValue { get; set; }
}
