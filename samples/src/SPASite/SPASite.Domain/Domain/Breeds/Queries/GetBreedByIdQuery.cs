namespace SPASite.Domain;

public class GetBreedByIdQuery : IQuery<Breed?>
{
    public GetBreedByIdQuery() { }

    public GetBreedByIdQuery(int id)
    {
        BreedId = id;
    }

    public int BreedId { get; set; }
}
