namespace Cofoundry.Domain.CQS;

/// <summary>
/// A command that can be patched, indicating that it has an
/// accompanying <see cref="GetPatchableCommandQuery"/> to hydrate
/// the command with existing data.
/// </summary>
public interface IPatchableCommand : ICommand
{
}
