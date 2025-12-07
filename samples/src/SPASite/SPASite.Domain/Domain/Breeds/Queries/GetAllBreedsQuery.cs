namespace SPASite.Domain;

/// <summary>
/// A query handler always requires a query definition, even if there are 
/// no parameters.
/// </summary>
public class GetAllBreedsQuery : IQuery<IEnumerable<Breed>>
{
}