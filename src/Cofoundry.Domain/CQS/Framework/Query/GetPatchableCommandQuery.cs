namespace Cofoundry.Domain.CQS;

/// <summary>
/// A generic query to get a pre-populated update command of the specified type.
/// This is typically used in as part of a API patch operation where the update 
/// command is first loaded and then modified with only the parameters that need
/// to be changed.
/// </summary>
/// <typeparam name="TCommand"><see cref="ICommand"/>type to fetch.</typeparam>
public class GetPatchableCommandQuery<TCommand>
    : IQuery<TCommand>
    where TCommand : IPatchableCommand
{
}
