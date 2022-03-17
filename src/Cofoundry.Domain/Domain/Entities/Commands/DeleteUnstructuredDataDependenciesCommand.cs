namespace Cofoundry.Domain;

public class DeleteUnstructuredDataDependenciesCommand : ICommand
{
    public DeleteUnstructuredDataDependenciesCommand(string rootEntityDefinitionCode, int rootEntityId)
    {
        RootEntityDefinitionCode = rootEntityDefinitionCode;
        RootEntityId = rootEntityId;
    }

    [Required]
    public string RootEntityDefinitionCode { get; set; }

    [Required]
    [PositiveInteger]
    public int RootEntityId { get; set; }
}
