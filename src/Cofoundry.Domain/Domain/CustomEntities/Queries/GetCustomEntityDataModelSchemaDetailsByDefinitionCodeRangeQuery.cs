namespace Cofoundry.Domain;

/// <summary>
/// Query to extract and return meta data information about a custom 
/// entity data model for a range of custom entity definitions.
/// </summary>
public class GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery : IQuery<IReadOnlyDictionary<string, CustomEntityDataModelSchema>>
{
    /// <summary>
    /// Query to extract and return meta data information about a custom 
    /// entity data model for a range of custom entity definitions.
    /// </summary>
    public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery()
    {
        CustomEntityDefinitionCodes = new List<string>();
    }

    /// <summary>
    /// Query to extract and return meta data information about a custom 
    /// entity data model for a range of custom entity definitions.
    /// </summary>
    /// <param name="customEntityDefinitionCodes">Range of definition codes to query (the unique 6 letter code representing the entity).</param>
    public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery(
        IEnumerable<string>? customEntityDefinitionCodes
        )
        : this(customEntityDefinitionCodes?.ToArray() ?? [])
    {
    }

    /// <summary>
    /// Query to extract and return meta data information about a custom 
    /// entity data model for a range of custom entity definitions.
    /// </summary>
    /// <param name="customEntityDefinitionCodes">Range of definition codes to query (the unique 6 letter code representing the entity).</param>
    public GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery(
        IReadOnlyCollection<string> customEntityDefinitionCodes
        )
    {
        ArgumentNullException.ThrowIfNull(customEntityDefinitionCodes);

        CustomEntityDefinitionCodes = customEntityDefinitionCodes;
    }

    /// <summary>
    /// Range of definition codes to query (the unique 6 letter code representing the entity).
    /// </summary>
    [Required]
    public IReadOnlyCollection<string> CustomEntityDefinitionCodes { get; set; }
}
