namespace Cofoundry.Domain;

public class UpdateUnstructuredDataDependenciesCommand : ICommand
{
    public UpdateUnstructuredDataDependenciesCommand(string rootEntityDefinitionCode, int rootEntityId, object model)
    {
        RootEntityDefinitionCode = rootEntityDefinitionCode;
        RootEntityId = rootEntityId;
        Model = model;
    }

    [Required]
    public string RootEntityDefinitionCode { get; set; }

    [Required]
    [PositiveInteger]
    public int RootEntityId { get; set; }

    [Required]
    public object Model { get; set; }
}
