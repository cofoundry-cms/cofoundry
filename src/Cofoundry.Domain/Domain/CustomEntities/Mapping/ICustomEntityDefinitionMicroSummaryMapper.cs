namespace Cofoundry.Domain.Internal;

/// <summary>
/// Simple mapper for mapping to CustomEntityDefinitionMicroSummary objects.
/// </summary>
public interface ICustomEntityDefinitionMicroSummaryMapper
{
    /// <summary>
    /// Maps a code base custom entity definition into a CustomEntityDefinitionMicroSummary object.
    /// </summary>
    /// <param name="codeDefinition">Code based definition to map.</param>
    CustomEntityDefinitionMicroSummary Map(ICustomEntityDefinition codeDefinition);

    /// <summary>
    /// Maps a CustomEntityDefinitionSummary into a CustomEntityDefinitionMicroSummary object.
    /// </summary>
    /// <param name="summary">Instance to map.</param>
    CustomEntityDefinitionMicroSummary Map(CustomEntityDefinitionSummary summary);
}
