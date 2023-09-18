namespace Cofoundry.Domain;

public class UpdateUnstructuredDataDependenciesCommand : ICommand
{
    public UpdateUnstructuredDataDependenciesCommand(string rootEntityDefinitionCode, int rootEntityId, params object[] models)
    {
        RootEntityDefinitionCode = rootEntityDefinitionCode;
        RootEntityId = rootEntityId;
        Model = models;
    }

    [Required]
    public string RootEntityDefinitionCode { get; set; }

    [Required]
    [PositiveInteger]
    public int RootEntityId { get; set; }

    [Required]
    public ICollection<object> Model { get; set; }
}
