namespace Cofoundry.Domain.CQS;

/// <summary>
/// A command that can be patched, indicating that it has an
/// accompanying <see cref="GetPatchableByIdCommandQuery"/> to hydrate
/// the command with existing data, using an identifying integer id to 
/// retreive the existing data.
/// </summary>
public interface IPatchableByIdCommand : ICommand
{
}
