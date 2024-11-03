namespace Cofoundry.Plugins.ErrorLogging.Domain;

public class GetErrorDetailsByIdQuery : IQuery<ErrorDetails?>
{
    public GetErrorDetailsByIdQuery() { }

    public GetErrorDetailsByIdQuery(int errorId)
    {
        ErrorId = errorId;
    }

    public int ErrorId { get; set; }
}
